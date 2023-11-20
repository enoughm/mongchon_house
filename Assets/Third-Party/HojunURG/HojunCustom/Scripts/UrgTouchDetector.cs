using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum UrgTouchState
{
    //아무것도 없는 상태
    Empty,
    //Touch가 스쳐가듯 된 상태
    TouchMoment,
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
    public Action<UrgTouchState, Vector2> HokuyoAction = null;
    public Action<string, UrgGridObserverData> RectObserveAction = null;
    
    
    public UrgControlCustom UrgControl => urgControl;
    public UrgDeviceEthernetCustom UrgDeviceEthernet => urgDeviceEthernetCustom;
    public UrgSensingCustom UrgSensing => urgSensing;
    public List<RealTouchData> AllScreenTouchList => _allScreenTouchList;
    public Vector2 TouchGrids => settingData.touchGrid;
   // public UrgTouchData[,] TouchGridItems => _touchGridItems;
    public UrgGridData[,] UrgGridItems => _urgGridArray;

    public List<UrgGridData> UrgGridItemsParsedToList => ChangeArrayToOneDimensionList(_urgGridArray);

    public UrgGridDataSetting[,] UrgGridDataSettingArray => _urgGridDataSettingArray;
    public Vector2 TouchGridCellSize => _touchGridCellSize;
    public int TargetDisplay => _targetDisplay;
    public int ScreenWidth { get; private set; }
    public  int ScreenHeight { get; private set; }
    
    public string HokuyoIP = "192.168.0.10";

    
    [Header("TARGET CAMERA")]
    [SerializeField] private Camera targetCam;
    
    //touch setting data
    [Header("SETTING DATA")]
    [SerializeField] private UrgTouchDetectorSettingData settingData;

    //urg components
    [Header("urg components")]
    [SerializeField] private UrgSensingCustom urgSensing;
    [SerializeField] private UrgControlCustom urgControl;
    [SerializeField] private UrgDeviceEthernetCustom urgDeviceEthernetCustom;

    [Header("debug and setting")]
    [SerializeField] private UrgTouchDetectorDebugCanvas debugCanvas;
    [SerializeField] private UrgTouchDetectorSettingCanvas settingCanvas;

    [Header("rect observe")]
    [SerializeField] private bool checkMouseViewPortPos;
    [SerializeField] private SerializedDictionary<string, Rect> registeredObserver;
    
    private List<RealTouchData> _allScreenTouchList = new List<RealTouchData>();
    
    //grid data 관련 
    private UrgGridData[,] _urgGridArray; //실제 그리드 데이터
    private UrgGridDataSetting[,] _urgGridDataSettingArray; //그리드별 저장하고 불러올 옵션 데이터
    private List<List<UrgGridData>> _gridDataAreaList = new List<List<UrgGridData>>();
    
    //자동세팅되는 변수들
    private int _targetDisplay;
    private Vector2 _touchGridCellSize;





    private void Awake()
    {
        _targetDisplay = targetCam.targetDisplay;
        ScreenWidth = targetCam.pixelWidth;
        ScreenHeight = targetCam.pixelHeight;

        CreateTouchGrid();
    }

    private void Start()
    {
        DataLoad();
        urgControl.StartTcp(HokuyoIP);
    }

    /// <summary>
    /// rect observe
    /// </summary>
    private void UpdateRect()
    {
        foreach (var item in registeredObserver)
        {
            var list = UrgGridItemsParsedToList.Where(data => item.Value.Contains(data.gridViewPortPos)).ToList();
            var total = list.Sum(data => data.sensedDataCountAverage);
            if (list.Count > 0 && total > 0)
            {
                UrgGridObserverData observerData = new UrgGridObserverData()
                {
                    data = new List<UrgGridData>(),
                    averageSum = 0,
                };
                observerData.data = list;
                observerData.averageSum = list.Sum(data => data.sensedDataCountAverage);
                RectObserveAction?.Invoke(item.Key, observerData);
            }
        }
        //옵저버 데이터를 Action으로 뺴준다
    }

    private void Update()
    {
        //디버그 및 세팅 UI 띄우기
        if(Input.GetKeyDown(KeyCode.Z))
            debugCanvas.gameObject.SetActive(!debugCanvas.gameObject.activeSelf);
        else if(Input.GetKeyDown(KeyCode.X))
            settingCanvas.gameObject.SetActive(!settingCanvas.gameObject.activeSelf);

        
        if (checkMouseViewPortPos)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 viewportPos = targetCam.ScreenToViewportPoint(mousePos);
            Debug.Log(viewportPos);
        }

            //grid data 초기화 (화면을 그리드로 나누고, 그리드 당 데이터의 개수를 확인하는)
        InitGridData();
        
        //실제 감지 영역안의 모든 감지 데이터를 받아온다.
        var allSensedDatas = urgSensing.convertedAllSensedObjsInRealArea;
        for (int i = 0; i < allSensedDatas.Count; i++)
        {
            var targetPos = GetGridPosFromViewportPos(allSensedDatas[i].viewPortPos);
            int x = Mathf.Clamp((int) targetPos.x, 0, (int) settingData.touchGrid.x-1);
            int y = Mathf.Clamp((int) targetPos.y, 0, (int) settingData.touchGrid.y-1);
            try
            {
                //받아온 데이터는 각 위치에 맞는 그리드데이터 안에 저장시킨다
                if(_urgGridDataSettingArray[x, y].isAvailableArea)
                    continue;
                _urgGridArray[x, y].sensedObjects.Add(allSensedDatas[i]);
            }
            catch (Exception e)
            {
                Debug.Log($"viewPortPos:{allSensedDatas[i].viewPortPos}");
                Debug.Log($"targetPos:{targetPos}, x:{x}, y:{y}");
                Debug.Log($"_urgGridArray [{_urgGridArray.GetLength(0)}, {_urgGridArray.GetLength(1)}]");
            }
        }
        
        //그리드를 모두 순회하면서 데이터를 가공한다.
        for (int i = 0; i < _urgGridArray.GetLength(0); i++)
        {
            for (int j = 0; j < _urgGridArray.GetLength(1); j++)
            {
                //주변 상하좌우대각선 8방향의 데이터를 리스트로 가져온다.
                var data = GetSurroundingData(_urgGridArray, i, j, 1);
                if (data.Count > 0)
                {
                    //주변 데이터를 저장
                    _urgGridArray[i, j].surroundingGridDataList = data;
                }

                
                var gridWidth = _touchGridCellSize.x / ScreenWidth;
                var gridHeight = _touchGridCellSize.y / ScreenHeight;
                
                //Grid Area에 대한 이다 (단위는 ViewPort.. 좌하단에서 0,0 ~ 우상단 1,1)
                _urgGridArray[i, j].gridViewPortRect = new Rect()
                {
                    x = i / settingData.touchGrid.x,
                    y = j / settingData.touchGrid.y,
                    width = gridWidth,
                    height = gridHeight,
                };
                //Grid 중심 포지션에 대한 값이다.
                _urgGridArray[i, j].gridViewPortPos = new Vector2(i / settingData.touchGrid.x + gridWidth, j / settingData.touchGrid.y + gridHeight);
                
                //한 그리드 영역에 있는 Sensor 데이터 개수를 넣어준다.
                _urgGridArray[i, j].sensedDataCountQueue.Enqueue(_urgGridArray[i, j].sensedObjects.Count);
                
                //한 그리드 영역에 있는 Sensor 데이터 개수의 평균을 구한다.
                _urgGridArray[i, j].sensedDataCountAverage = GetCountQueueAverage(ref _urgGridArray[i, j].sensedDataCountQueue, settingData.gridAverageMaxCollectCount);
                
                //Sensor 데이터 개수가 무시 가능한 값보다 크다면, 데이터가 있는 것으로 판단한다.
                if (_urgGridArray[i, j].sensedDataCountAverage > settingData.gridAverageAcceptThreshold)
                    _urgGridArray[i, j].isAnyDataGrid = true;
                 
                //0보다 큰 경우 이 그리드는 무시되었다.
                else if(_urgGridArray[i, j].sensedDataCountAverage > 0)
                    _urgGridArray[i, j].isIgnoredGrid = true;
            }   
        }
        
        //그리드 데이터의 영역을 기반해서 데이터를 다시 가공한다
        //등고선 데이터와 비슷하게, 데이터가 모여있는 지점에서 끝나는 영역까지의 모든 Grid 영역을 하나의 단위로 하여 아이템 항목을 가져온다
        //재귀함수를 이용함
        _gridDataAreaList = new List<List<UrgGridData>>();
        for (int i = 0; i < _urgGridArray.GetLength(0); i++)
        {
            for (int j = 0; j < _urgGridArray.GetLength(1); j++)
            {
                if (_urgGridArray[i, j].sensedDataCountAverage < settingData.gridAverageAcceptThreshold || _urgGridArray[i, j].linkedGridCheck) {
                    continue;
                }
                var item = GetLinkedUrgGridDatas(i, j);
                if (item.Count > settingData.gridAreaCountCheckThreshold)
                {
                    //Debug.Log(item.Count);
                    _gridDataAreaList.Add(item);
                }
            }   
        }

        
        //여기서는 기존 데이터를 가공하여 가장 높은 포인트(센서감지데이터 개수)를 가진 그리드를 찾는다.
        for (int i = 0; i < _gridDataAreaList.Count; i++)
        {
            //데이터 개수가 임계값을 넘는 경우만 리스트로 가져온다.
            List<UrgGridData> linkedHighAverageGridDatas = new List<UrgGridData>();
            linkedHighAverageGridDatas = _gridDataAreaList[i].Where(data => data.sensedDataCountAverage > settingData.gridAverageAcceptThreshold).ToList();
            
            //그리드의 위치가 높은 순으로 정렬한다. -type1
            //linkedHighAverageGridDatas = linkedHighAverageGridDatas.OrderByDescending(o => o.gridPos.y).ToList();
            //var item = linkedHighAverageGridDatas.First();
            
            
            //그리드의 평균이 높은 순으로 정렬한다. - type2
            linkedHighAverageGridDatas = linkedHighAverageGridDatas.OrderByDescending(o => o.sensedDataCountAverage).ToList();
            var item = linkedHighAverageGridDatas.First();
            
            //여기서부터는 그리드 간 데이터가 근소하여, 경합이 발생하는 경우를 처리한다.
            //todo: 최대값의 비율만큼 경합조건 붙일 수 있도록 추가
            bool isCompetition = false;
            List<UrgGridData> competitionList = new List<UrgGridData>();
            competitionList.Add(item);
            for (int j = 1; j < linkedHighAverageGridDatas.Count; j++)
            {
                var percentage = item.sensedDataCountAverage * settingData.competitionApproveThreshold;
                if (Mathf.Abs(item.sensedDataCountAverage - linkedHighAverageGridDatas[j].sensedDataCountAverage) < percentage)
                {
                    //Debug.Log($"경합: {item.average}, {list[j].average}");
                    isCompetition = true;
                    competitionList.Add(linkedHighAverageGridDatas[j]);
                }
            }
            
            //경합이 있었을 경우에는 경합한 그리드 중에서 가장 y값과 x값이 높은 그리드를 선택한다.
            if (isCompetition)
            {
                var xList = competitionList.OrderByDescending(o => o.gridPos.y).ToList();
                var groupBy = xList.GroupBy(o => o.gridPos.y).ToList();
                var finalList = groupBy.First().OrderByDescending(o => o.gridPos.x).ToList();
                
                
                item = finalList.First();
                //Debug.Log($"승리: {item.gridPos.x}, {item.gridPos.y}");
            }

            //경쟁했는지 데이터 추가
            for (int j = 0; j < competitionList.Count; j++)
                _urgGridArray[(int)competitionList[j].gridPos.x, (int)competitionList[j].gridPos.y].isCompetitionGrid = true;
            
            //결과값은 데이터에 추가된다.
            //Debug.Log($"item.gridPos.x : {item.gridPos.x}, item.gridPos.y : {item.gridPos.y}");
            _urgGridArray[(int)item.gridPos.x, (int)item.gridPos.y].isHighAverageGrid = true;
        }
        
        //todo: Grid Observe 방식의 그리드 추가
        InitTouchData();
        for (int i = 0; i < _gridDataAreaList.Count; i++)
        {
            List<UrgGridData> area = new List<UrgGridData>();
            area = _gridDataAreaList[i];
            List<Vector2> midPointList = new List<Vector2>();

            for (int j = 0; j < area.Count; j++)
            {
                var item = area[j];
                if(_urgGridArray[(int)item.gridPos.x, (int)item.gridPos.y].isCompetitionGrid)
                     midPointList.Add(_urgGridArray[(int)item.gridPos.x, (int)item.gridPos.y].gridViewPortPos);
            }

            var vector2 = GetMidPoint(midPointList);
            InsertUrgTouchData(vector2);
        }
        
        RemoveUrgTouchData();

        //touch data 초기화
        // InitTouchData();
        // var datas = urgSensing.convertedSensedObjsInRealArea;
        // for (int i = 0; i < datas.Count; i++)
        // {
        //     InsertUrgTouchData(datas[i].viewPortPos);
        // }
        // RemoveUrgTouchData();

        UpdateRect();
    }
    
     private void CreateTouchGrid()
    {
        var cellWidth = ScreenWidth / settingData.touchGrid.x;
        var cellHeight = ScreenHeight / settingData.touchGrid.y;
        _touchGridCellSize.x = cellWidth;
        _touchGridCellSize.y = cellHeight;
       // _touchGridItems = new UrgTouchData[(int) settingData.touchGrid.x, (int) settingData.touchGrid.y];
       _urgGridDataSettingArray = new UrgGridDataSetting[(int) settingData.touchGrid.x, (int) settingData.touchGrid.y];
        _urgGridArray = new UrgGridData[(int) settingData.touchGrid.x, (int) settingData.touchGrid.y];
        for (int x = 0; x < (int) settingData.touchGrid.x; x++)
        {
            for (int y = 0; y < (int) settingData.touchGrid.y; y++)
            {
                //그리드 데이터
                _urgGridArray[x,y].sensedObjects = new List<UrgSensingCustom.ConvertedSensedObject>();
                _urgGridArray[x,y].surroundingGridDataList = new List<UrgGridData>();
                _urgGridArray[x,y].sensedDataCountQueue = new Queue<float>();
                _urgGridArray[x, y].gridPos = new Vector2(x, y);
                
                
                //그리드 세팅 데이터
                _urgGridDataSettingArray[x, y].gridPos = new Vector2(x, y);
                _urgGridDataSettingArray[x, y].isAvailableArea = false;
            }
        }
    }

    private void DataLoad()
    {
        HokuyoIP = PlayerPrefs.GetString($"{_targetDisplay}_hokuyoIP", HokuyoIP);
        urgSensing.sensingAreaSize.x = PlayerPrefs.GetFloat($"{_targetDisplay}_screenAreaSizeX", urgSensing.sensingAreaSize.x);
        urgSensing.sensingAreaSize.y = PlayerPrefs.GetFloat($"{_targetDisplay}_screenAreaSizeY", urgSensing.sensingAreaSize.y);
        urgSensing.sensingAreaOffset.x = PlayerPrefs.GetFloat($"{_targetDisplay}_screenAreaOffsetX", urgSensing.sensingAreaOffset.x);
        urgSensing.sensingAreaOffset.y = PlayerPrefs.GetFloat($"{_targetDisplay}_screenAreaOffsetY", urgSensing.sensingAreaOffset.y);
        urgSensing.actuallySensingAreaSize.x = PlayerPrefs.GetFloat($"{_targetDisplay}_realAreaSizeX", urgSensing.actuallySensingAreaSize.x);
        urgSensing.actuallySensingAreaSize.y = PlayerPrefs.GetFloat($"{_targetDisplay}_realAreaSizeY", urgSensing.actuallySensingAreaSize.y);
        urgSensing.actuallySensingAreaOffset.x = PlayerPrefs.GetFloat($"{_targetDisplay}_realAreaOffsetX", urgSensing.actuallySensingAreaOffset.x);
        urgSensing.actuallySensingAreaOffset.y = PlayerPrefs.GetFloat($"{_targetDisplay}_realAreaOffsetY", urgSensing.actuallySensingAreaOffset.y);
        urgSensing.objThreshold = PlayerPrefs.GetFloat($"{_targetDisplay}_objThreshold", urgSensing.objThreshold);
        urgSensing.minWidth = PlayerPrefs.GetFloat($"{_targetDisplay}_minWidth", urgSensing.minWidth);
        LoadGridDataSettingArray();
    }
    
    public void DataSave()
    {
        PlayerPrefs.SetString($"{_targetDisplay}_hokuyoIP", HokuyoIP);
        PlayerPrefs.SetFloat($"{_targetDisplay}_screenAreaSizeX", urgSensing.sensingAreaSize.x);
        PlayerPrefs.SetFloat($"{_targetDisplay}_screenAreaSizeY", urgSensing.sensingAreaSize.y);
        PlayerPrefs.SetFloat($"{_targetDisplay}_screenAreaOffsetX", urgSensing.sensingAreaOffset.x);
        PlayerPrefs.SetFloat($"{_targetDisplay}_screenAreaOffsetY", urgSensing.sensingAreaOffset.y);
        PlayerPrefs.SetFloat($"{_targetDisplay}_realAreaSizeX", urgSensing.actuallySensingAreaSize.x);
        PlayerPrefs.SetFloat($"{_targetDisplay}_realAreaSizeY", urgSensing.actuallySensingAreaSize.y);
        PlayerPrefs.SetFloat($"{_targetDisplay}_realAreaOffsetX", urgSensing.actuallySensingAreaOffset.x);
        PlayerPrefs.SetFloat($"{_targetDisplay}_realAreaOffsetY", urgSensing.actuallySensingAreaOffset.y);
        PlayerPrefs.SetFloat($"{_targetDisplay}_objThreshold", urgSensing.objThreshold);
        PlayerPrefs.SetFloat($"{_targetDisplay}_minWidth", urgSensing.minWidth);
        SaveGridDataSettingArray();
    }

    public void SetGridDataSetting(Vector2 arg1, bool arg2)
    {
        UrgGridDataSettingArray[(int)arg1.x, (int)arg1.y].isAvailableArea = arg2;
    }
    
    Vector2 GetMidPoint(List<Vector2> vectorList)
    {
        if (vectorList.Count == 0)
        {
            throw new ArgumentException("vectorList에 값이 없습니다.");
        }

        float sumX = 0;
        float sumY = 0;

        foreach (Vector2 vector in vectorList)
        {
            sumX += vector.x;
            sumY += vector.y;
        }

        float midX = sumX / vectorList.Count;
        float midY = sumY / vectorList.Count;

        return new Vector2(midX, midY);
    }

    /// <summary>
    /// 재귀함수이다, 
    /// </summary>
    private List<UrgGridData> GetLinkedUrgGridDatas(int row, int col)
    {
        //상 하 좌 우로 쭉 가면서 0이거나 ignorevalue보다 average가 작을 때까지 Max인 포인터를 가지고 있게한다
        List<UrgGridData> canValue = new List<UrgGridData>();
        if(_urgGridArray[row, col].linkedGridCheck)
            return canValue;
        
        //자기자신 체크 및 추가
        _urgGridArray[row, col].linkedGridCheck = true;
        if (_urgGridArray[row, col].sensedDataCountAverage > settingData.gridAverageAcceptThreshold)
            canValue.Add(_urgGridArray[row, col]);
        
        
        //주변 데이터 조회 (가져올 값이 있는지 찾는다)
        var surroundingData = _urgGridArray[row, col].surroundingGridDataList;
        
        //주변 9칸에 대해서 찾는다
        for (int i = 0; i < surroundingData.Count; i++)
        {
            int x = (int)surroundingData[i].gridPos.x;
            int y = (int)surroundingData[i].gridPos.y;
            if (_urgGridArray[x, y].sensedDataCountAverage > settingData.gridAverageAcceptThreshold && !_urgGridArray[x, y].linkedGridCheck)
            {
                var list = GetLinkedUrgGridDatas(x, y);
                if(list.Count > 0)
                    canValue.AddRange(list);;
            }
        }

        return canValue;
    }


    private void InitGridData()
    {
        for (int x = 0; x < _urgGridArray.GetLength(0); x++)
        {
            for (int y = 0; y < _urgGridArray.GetLength(1); y++)
            {
                _urgGridArray[x, y].surroundingGridDataList.Clear();
                _urgGridArray[x, y].sensedObjects.Clear();
                _urgGridArray[x, y].sensedDataCountAverage = 0;
                _urgGridArray[x, y].linkedGridCheck = false;
                _urgGridArray[x, y].isHighAverageGrid = false;
                _urgGridArray[x, y].isIgnoredGrid = false;
                _urgGridArray[x, y].isCompetitionGrid = false;
                _urgGridArray[x, y].isAnyDataGrid = false;
            }
        }
    }
    
    // 탐지된 데이터를 이용해 탐지 영역 주변의 데이터를 조회하는 함수
    private List<UrgGridData> GetSurroundingData(UrgGridData[,] gridData, int x, int y, int depth)
    {
        List<UrgGridData> surroundingData = new List<UrgGridData>();

        // 주변 좌표 계산
        int startX = Mathf.Max(0, x - depth);
        int startY = Mathf.Max(0, y - depth);
        int endX = Mathf.Min(gridData.GetLength(0) - 1, x + depth);
        int endY = Mathf.Min(gridData.GetLength(1) - 1, y + depth);

        // 주변 데이터 추가
        for (int i = startX; i <= endX; i++)
        {
            for (int j = startY; j <= endY; j++)
            {
                if (i == x && j == y)
                    continue; // 현재 좌표인 경우 건너뜁니다

                surroundingData.Add(gridData[i, j]);
            }
        }

        return surroundingData;
    }


    private float GetCountQueueAverage(ref Queue<float> queue, int max)
    {
        while (queue.Count > max)
            queue.Dequeue();

        return queue.Average();
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
    
    private void InsertUrgTouchData(Vector2 sensedViewPortPos)
    {
        
        Vector2 viewPortPos = sensedViewPortPos;
        bool inserted = false;
        for (int i = 0; i < _allScreenTouchList.Count; i++)
        {
            if (inserted)
                break;
            
            var item = _allScreenTouchList[i];
            var isScreenEdge = IsScreenEdgePosition(item.viewPortPos);

            // 한 명의 경우라면 아래 코드가 맞음
            // item.viewPortPos = Vector2.Lerp(_allScreenTouchList[i].viewPortPos, viewPortPos, Time.deltaTime);
            // item.liveTime += Time.deltaTime;
            // item.emptyTime = 0;
            // item.updated = true;
            // inserted = true;
            
            //TEST
            if (Mathf.Abs(item.viewPortPos.x - viewPortPos.x) < settingData.touchMergeDistanceX && Mathf.Abs(item.viewPortPos.y - viewPortPos.y) < settingData.touchMergeDistanceY)
            {
                item.viewPortPos = Vector2.Lerp(_allScreenTouchList[i].viewPortPos, viewPortPos, Time.deltaTime * settingData.touchCheckSpeed);
                item.liveTime += Time.deltaTime;
                item.emptyTime = 0;
                item.updated = true;
                inserted = true;
            }

            switch (item.touchState)
            {
                case UrgTouchState.Empty:
                    break;
                case UrgTouchState.TouchMoment:
                    if(item.liveTime > settingData.touchDownCheckDuration)
                        item.touchState = UrgTouchState.TouchDown;
                    break;
                case UrgTouchState.TouchDown:
                    item.touchState = UrgTouchState.TouchPress;
                    break;
                case UrgTouchState.TouchPress:
                    break;
                case UrgTouchState.TouchPressUp:
                    break;
            }
            _allScreenTouchList[i] = item;
            
            
            //INVOKE
            HokuyoAction?.Invoke(item.touchState, item.viewPortPos);
        }

        if (!inserted)
        {
            _allScreenTouchList.Add(new RealTouchData()
            {
                touchState = UrgTouchState.TouchMoment,
                viewPortPos = viewPortPos,
                initPos = viewPortPos,
                liveTime = 0,
                emptyTime = 0,
                updated = true,
            });
        }
    }

    private void RemoveUrgTouchData()
    {
        for (int i = 0; i < _allScreenTouchList.Count; i++)
        {
            var item = _allScreenTouchList[i];
            //bool isEdge = IsEdgePosition(item.viewPortPos);
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
                if (item.emptyTime > settingData.touchCheckingDuration)
                {
                    //if (item.liveTime < settingData.touchInvokeDuration || merged || isEdge || isScreenEdge)
                    if (item.liveTime < settingData.touchInvokeDuration || merged || isScreenEdge || item.touchState != UrgTouchState.TouchPress)
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
    
    private Vector2 GetGridPosFromViewportPos(Vector2 viewPortPos)
    {
        var x = viewPortPos.x * ScreenWidth;
        var y = viewPortPos.y * ScreenHeight;
        var idxX = (int) (x / _touchGridCellSize.x);
        var idxY = (int) (y / _touchGridCellSize.y);
        return new Vector2(idxX, idxY);
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


    private void SaveGridDataSettingArray()
    {
        try
        {
            UrgGridDataSettingSave save = new UrgGridDataSettingSave();

            for (int i = 0; i < _urgGridDataSettingArray.GetLength(0); i++)
            {
                for (int j = 0; j < _urgGridDataSettingArray.GetLength(1); j++)
                {
                    save.list.Add(_urgGridDataSettingArray[i, j]);
                }
            }

            string json = JsonUtility.ToJson(save, true);
            Debug.Log(json);

            // 여기에서 json 문자열을 파일에 저장 또는 필요한 대로 사용할 수 있습니다.
            PlayerPrefs.SetString($"{_targetDisplay}_urgGridDataSettingArray", json);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private List<T> ChangeArrayToOneDimensionList<T>(T[,] t)
    {
        var ret = new List<T>();
        
        int rows = t.GetLength(0);
        int columns = t.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                ret.Add(t[i, j]);
            }
        }
        return ret;
    }
    
    private void LoadGridDataSettingArray()
    {
        try
        {
            // JSON 파일에서 문자열 읽어오는 예시
            bool has = PlayerPrefs.HasKey($"{_targetDisplay}_urgGridDataSettingArray");
            if (!has)
                return;
            string json = PlayerPrefs.GetString($"{_targetDisplay}_urgGridDataSettingArray");
            var list2D = JsonUtility.FromJson<UrgGridDataSettingSave>(json);

            UrgGridDataSetting[,] newArray2D = new UrgGridDataSetting[_urgGridDataSettingArray.GetLength(0), _urgGridDataSettingArray.GetLength(1)];

            for (int i = 0; i < _urgGridDataSettingArray.GetLength(0); i++)
            {
                for (int j = 0; j < _urgGridDataSettingArray.GetLength(1); j++)
                {
                    newArray2D[i, j] = list2D.list[i * _urgGridDataSettingArray.GetLength(1) + j];
                }
            }

            _urgGridDataSettingArray = newArray2D;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}

[System.Serializable]
public class UrgGridDataSettingSave
{
    public List<UrgGridDataSetting> list = new List<UrgGridDataSetting>();
}

[System.Serializable]
public struct UrgGridDataSetting
{
    public Vector2 gridPos;
    public bool isAvailableArea;
}

public struct UrgGridObserverData
{
    public List<UrgGridData> data;
    public float averageSum;
}

public struct UrgGridData
{
    //그리드 영역에 대한 정보
    public Vector2 gridPos;
    public Vector2 gridViewPortPos;
    public Rect gridViewPortRect;
    
    //그리드에 감지된 센서 데이터의 개수 평균 및 평균을 구하기 위한 Queue
    public List<UrgSensingCustom.ConvertedSensedObject> sensedObjects;
    public float sensedDataCountAverage;
    public Queue<float> sensedDataCountQueue;
    
    //제일 높은 개수의 그리드항목이다
    public bool isHighAverageGrid;
    public bool isCompetitionGrid;
    public bool isAnyDataGrid;
    public bool isIgnoredGrid;
    
    //주변 그리드에 대한 데이타
    public List<UrgGridData> surroundingGridDataList;
    
    //연결된 그리드를 가져오는데 사용됐는지.
    public bool linkedGridCheck;
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