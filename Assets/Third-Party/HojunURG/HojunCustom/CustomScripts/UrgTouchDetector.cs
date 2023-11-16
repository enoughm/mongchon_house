using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public enum UrgTouchState
{
    //아무것도 없는 상태
    Empty,
    //Touch된 상태
    TouchDown,
    //Touch된 상태에서 일정 시간이 더 지난 상태
    TouchPress,
    //Touch Click 상태 -> 이후에는 터치 이벤트 종료, data가 들어오지 않는 상태가 한 번은 돼야 터치가 들어올 수 있다.
    TouchPressUp,
    TouchClicked,
}

public class UrgTouchDetector : MonoBehaviour
{
    public List<RealTouchData> AllScreenTouchList => _allScreenTouchList;
    public Vector2 TouchGrids => settingData.touchGrid;
    public UrgTouchData[,] TouchGridItems => _touchGridItems;
    public Vector2 TouchGridCellSize => _touchGridCellSize;
    public float TouchForceEmptyDuration => touchForceEmptyDuration;
    public int ScreenWidth { get; private set; }
    public  int ScreenHeight { get; private set; }

    [SerializeField] private UrgTouchDetectorSettingData settingData;

    [SerializeField] private int targetDisplay;
    [SerializeField] private UrgSensing urgSensing;
    
    //한 그리드 내에서, 어느 정도 비율의 Edge 부분을 무시할지

    
    List<RealTouchData> _allScreenTouchList = new List<RealTouchData>();

    
    /// <summary>
    /// 얼마동안 터치를 땠을 경우 터치 취소시킬지
    /// </summary>
    [SerializeField] private List<UrgTouchStateSettingData> urgTouchStateData = new()
    {
        new() {touchState = UrgTouchState.Empty, nextStateInvokeDuration = 0.5f},
        new() {touchState = UrgTouchState.TouchDown, nextStateInvokeDuration = 0.05f},
        new() {touchState = UrgTouchState.TouchPress, nextStateInvokeDuration = 0.10f},
        new() {touchState = UrgTouchState.TouchPressUp, nextStateInvokeDuration = 0.2f},
        
    };
    [SerializeField] private float touchForceEmptyDuration = 0.5f;

    private UrgTouchData[,] _touchGridItems;
    private Vector2 _touchGridCellSize;

    
    private void Awake()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        
        ScreenWidth = Display.displays[targetDisplay].renderingWidth;
        ScreenHeight = Display.displays[targetDisplay].renderingHeight;
        CreateTouchGrid();
    }
    
    

    private void Update()
    {
        var datas = urgSensing.convertedScreenSensedObjs;
        for (int i = 0; i < datas.Count; i++)
        {
            var targetPos = GetGridPosFromViewportPos(datas[i].viewPortPos);
            int x = Mathf.Clamp((int) targetPos.x, 0, (int) settingData.touchGrid.x - 1);
            int y = Mathf.Clamp((int) targetPos.y, 0, (int) settingData.touchGrid.y - 1);
            try
            {
                _touchGridItems[x, y].gridRect = new Rect()
                {
                    x = x / settingData.touchGrid.x,
                    y = y / settingData.touchGrid.y,
                    width = _touchGridCellSize.x / ScreenWidth,
                    height = _touchGridCellSize.y / ScreenHeight,
                };
                _touchGridItems[x, y].sensedObject = datas[i];
                _touchGridItems[x, y].dataInserted = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
        
        InitTouchData();

        for (int x = 0; x < settingData.touchGrid.x; x++)
        {
            for (int y = 0; y < settingData.touchGrid.y; y++)
            {
                var item = _touchGridItems[x, y];
                var state = item.touchState;
                var touchStateSettingData = GetDurationValueFromTouchState(state);
                var isEdge = IsEdgePosition(item.sensedObject.viewPortPos);
                var isScreenEdge = IsScreenEdgePosition(item.sensedObject.viewPortPos);
                
        
                //time 계산
                if (item.dataInserted)
                {
                    item.InsertTime += Time.deltaTime;
                    item.EmptyTime = 0;
                }
                else
                {
                    
                    item.EmptyTime += Time.deltaTime;
                    item.InsertTime = 0;
                }
        
                
                switch (state)
                {
                    case UrgTouchState.Empty:
                        if (item.InsertTime > touchStateSettingData.nextStateInvokeDuration)
                        {
                            if (isEdge)
                            {
                                ChangeTouchState(ref item, UrgTouchState.TouchPress);
                            }
                            else
                            {
                                ChangeTouchState(ref item, UrgTouchState.TouchDown);
                            }
                        }
                        break;
                    case UrgTouchState.TouchDown:
                        ChangeTouchState(ref item, UrgTouchState.TouchPress);
                        break;
                    case UrgTouchState.TouchPress:
                        InsertUrgTouchData(item);
                        if(item.prevInsertTime > touchStateSettingData.nextStateInvokeDuration && item.InsertTime == 0 && !isEdge)
                            ChangeTouchState(ref item, UrgTouchState.TouchPressUp);
                        break;
                    case UrgTouchState.TouchPressUp:
                        ChangeTouchState(ref item, UrgTouchState.Empty);
                        break;
                }
                
                if (item.EmptyTime > touchForceEmptyDuration)
                {
                    ChangeTouchState(ref item, UrgTouchState.Empty);
                }
                
                //초기화
                item.dataInserted = false;
                item.prevInsertTime = item.InsertTime;
                _touchGridItems[x, y] = item;
            }
        }
        
        
        RemoveUrgTouchData();
    }

    private void InsertUrgTouchData(UrgTouchData data)
    {
        
        Vector2 viewPortPos = data.sensedObject.viewPortPos;
        bool inserted = false;
        for (int i = 0; i < _allScreenTouchList.Count; i++)
        {
            if (inserted)
                break;
            
            var item = _allScreenTouchList[i];

            
            switch (item.touchState)
            {
                case UrgTouchState.Empty:
                case UrgTouchState.TouchPressUp:
                case UrgTouchState.TouchClicked:
                    continue;
            }


            //그리드 영역안에서만 한 개로 합칠 것인지
            if (settingData.touchCheckInGridArea)
            {
                //check is in rect
                bool isIn = data.gridRect.Contains(item.viewPortPos);
                //Debug.Log($"xMin:{data.gridRect.xMin}, xMax:{data.gridRect.xMax}, yMin:{data.gridRect.yMin}, yMax:{data.gridRect.yMax}");
                //Debug.Log($"item.viewPortPos:{item.viewPortPos}");
                if (isIn)
                {
                    item.viewPortPos = Vector2.Lerp(_allScreenTouchList[i].viewPortPos, viewPortPos, Time.deltaTime * settingData.touchCheckSpeed);
                    item.touchState = UrgTouchState.TouchPress;
                    item.liveTime += Time.deltaTime;
                    item.emptyTime = 0;
                    item.updated = true;
                    inserted = true;
                }
                //if(item.viewPortPos.x > data.gridRect.xMin && item.viewPortPos.y > data.gridRect.yMin && item.viewPortPos.x < data.gridRect.xMax)
            }
            //전체 스크린의 거리 기준으로 합칠 것인지
            else
            {
                if (Mathf.Abs(item.viewPortPos.x - viewPortPos.x) < settingData.touchMergeDistanceX && Mathf.Abs(item.viewPortPos.y - viewPortPos.y) < settingData.touchMergeDistanceY)
                {
                    item.viewPortPos = Vector2.Lerp(_allScreenTouchList[i].viewPortPos, viewPortPos, Time.deltaTime * settingData.touchCheckSpeed);
                    item.touchState = UrgTouchState.TouchPress;
                    item.liveTime += Time.deltaTime;
                    item.emptyTime = 0;
                    item.updated = true;
                    inserted = true;
                }
            }
            
            _allScreenTouchList[i] = item;
        }

        if (!inserted)
        {
            _allScreenTouchList.Add(new RealTouchData()
            {
                touchState = UrgTouchState.TouchDown,
                viewPortPos = viewPortPos,
                initPos = viewPortPos,
                liveTime = 0,
                emptyTime = 0,
                updated = true,
            });
        }
    }

    private void InitTouchData()
    {
        List<int> removeIdx = new List<int>();
        for (int i = 0; i < _allScreenTouchList.Count; i++)
        {
            if(_allScreenTouchList[i].touchState == UrgTouchState.TouchClicked || _allScreenTouchList[i].touchState == UrgTouchState.TouchPressUp || _allScreenTouchList[i].touchState == UrgTouchState.Empty)
                removeIdx.Add(i);
        }
        for (int i = removeIdx.Count - 1; i >= 0; i--)
        {
            _allScreenTouchList.RemoveAt(removeIdx[i]);
        }
    }

    private void RemoveUrgTouchData()
    {
        for (int i = 0; i < _allScreenTouchList.Count; i++)
        {
            var item = _allScreenTouchList[i];
            bool isEdge = IsEdgePosition(item.viewPortPos);
            var isScreenEdge = IsScreenEdgePosition(item.viewPortPos);

            
            bool merged = false;
            for (int j = 0; j < _allScreenTouchList.Count; j++)
            {
                if(i == j)
                    continue;
                
                var target = _allScreenTouchList[j];

                if (Mathf.Abs(item.viewPortPos.x - target.viewPortPos.x) < settingData.touchMergeDistanceXForMerge && Mathf.Abs(item.viewPortPos.y - target.viewPortPos.y) < settingData.touchMergeDistanceYForMerge)
                {
                    merged = true;
                }
            }
            
            
            if (!item.updated )
            {
                item.emptyTime += Time.deltaTime;
                if (item.emptyTime > settingData.touchCancelDuration)
                {
                    if (item.liveTime < settingData.touchInvokeDuration || merged || isEdge || isScreenEdge)
                    {
                        item.touchState = UrgTouchState.Empty;
                    }
                    else if (Vector2.Distance(item.viewPortPos, item.initPos) < settingData.touchClickableDistance)
                    {
                        item.touchState = UrgTouchState.TouchClicked;
                    }
                    else
                    {
                        item.touchState = UrgTouchState.TouchPressUp;
                    }
                }
            }

            item.updated = false;
            _allScreenTouchList[i] = item;
        }
    }
    
    private void ChangeTouchState(ref UrgTouchData data, UrgTouchState state)
    {
        data.touchState = state;
        data.InsertTime = 0;
    }

    private Vector2 GetGridPosFromViewportPos(Vector2 viewPortPos)
    {
        var x = viewPortPos.x * ScreenWidth;
        var y = viewPortPos.y * ScreenHeight;
        var idxX = (int) (x / _touchGridCellSize.x);
        var idxY = (int) (y / _touchGridCellSize.y);
        return new Vector2(idxX, idxY);
    }
    
    private bool IsEdgePosition(Vector2 viewPortPos)
    {
        var curX = viewPortPos.x * ScreenWidth;
        var curY = viewPortPos.y * ScreenHeight;
        var idxX = (int) (curX / _touchGridCellSize.x);
        var idxY = (int) (curY / _touchGridCellSize.y);
        
        float minX = _touchGridCellSize.x * idxX;
        float maxX = _touchGridCellSize.x * idxX + _touchGridCellSize.x;

        float ignoreX = _touchGridCellSize.x * settingData.ignorePaddingXRatio;
        
        if(curX < minX + ignoreX || curX > maxX - ignoreX)
            return true;
        
        float minY = _touchGridCellSize.y * idxY;
        float maxY = _touchGridCellSize.y * idxY + _touchGridCellSize.y;
        float ignoreY = _touchGridCellSize.y * settingData.ignorePaddingYRatio;

        if(curY < minY + ignoreY || curY > maxY - ignoreY)
            return true;
        
        return false;
    }

    private bool IsScreenEdgePosition(Vector2 viewPortPos)
    {
        var curX = viewPortPos.x * ScreenWidth;
        var curY = viewPortPos.y * ScreenHeight;
        float ignoreX = ScreenWidth * settingData.ignoreScreenPaddingXRatio;
        float ignoreY = ScreenHeight * settingData.ignoreScreenPaddingYRatio;

        if(curX < ignoreX || curX > ScreenWidth - ignoreX)
            return true;

        if(curY < ignoreY || curY > ScreenHeight - ignoreY)
            return true;
        
        return false;
    }

    private void CreateTouchGrid()
    {
        var cellWidth = ScreenWidth / settingData.touchGrid.x;
        var cellHeight = ScreenHeight / settingData.touchGrid.y;
        _touchGridCellSize.x = cellWidth;
        _touchGridCellSize.y = cellHeight;
        _touchGridItems = new UrgTouchData[(int) settingData.touchGrid.x, (int) settingData.touchGrid.y];
    }

    private UrgTouchStateSettingData GetDurationValueFromTouchState(UrgTouchState state)
    {
        return urgTouchStateData.Find(data => data.touchState == state);
    }

    [System.Serializable]
    public class UrgTouchStateSettingData
    {
        public UrgTouchState touchState;
        public float nextStateInvokeDuration;
    }
}


public struct UrgTouchData
{
    public UrgSensing.ConvertedSensedObject sensedObject;
    public UrgTouchState touchState;
    public Rect gridRect;
    public bool dataInserted;

    public float EmptyTime
    {
        get => _emptyTime;
        set => _emptyTime = Mathf.Clamp(value, 0, float.MaxValue);
    }
    public float InsertTime
    {
        get => _insertTime;
        set => _insertTime = Mathf.Clamp(value, 0, float.MaxValue);
    }

    public float prevInsertTime;
    private float _insertTime;
    private float _emptyTime;
}


[System.Serializable]
public struct RealTouchData
{
    public Vector2 viewPortPos;
    public UrgTouchState touchState;
    public float liveTime;
    public float emptyTime;
    public Vector2 initPos;
    public bool updated;
}