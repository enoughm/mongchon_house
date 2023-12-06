using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera FloorCamera
    {
        get
        {
            if(_floorCamera == null)
                _floorCamera = GameObject.FindGameObjectWithTag("FloorCam").GetComponent<Camera>();
            return _floorCamera;
        }
    }
    
    public Camera WallCamera
    {
        get
        {
            if(_wallCamera == null)
                _wallCamera = GameObject.FindGameObjectWithTag("WallCam").GetComponent<Camera>();
            return _wallCamera;
        }
    }
    
    public UrgTouchDetector WallUrgTouchDetector => _wallUrgTouchDetector;


    public bool IsSomeone => _isSomeone;
    public int FloorTouchCount => _floorUrgTouchDetector.AllScreenTouchList.Count;
    public UrgTouchDetector FloorUrgTouchDetector => _floorUrgTouchDetector;
    public List<GameObject> StepList => _stepList;

    
    
    public float floorEmptyTimeMax = 10;
    public float floorInvokeTimeMax = 3;
    
    
    UrgTouchDetector _floorUrgTouchDetector;
    UrgTouchDetector _wallUrgTouchDetector;

    private bool _isSomeone = false;
    private float _floorInvokeTime = 0;
    private float _floorEmptyTime = 0;
    private Camera _floorCamera;
    private List<GameObject> _stepList = new List<GameObject>();

    
    private Camera _wallCamera;

    
    private void Awake()
    {
        //find tag
        //_floorUrgTouchDetector = GameObject.FindGameObjectWithTag("FloorUrgTouchDetector").GetComponent<UrgTouchDetector>();
        _wallUrgTouchDetector = GameObject.FindGameObjectWithTag("WallUrgTouchDetector").GetComponent<UrgTouchDetector>();
    }

    private void OnEnable()
    {
        WallUrgTouchDetector.RectObserveAction += OnWallUrgTouchDetectorRectObserveAction;
    }

    private void OnDisable()
    {
        WallUrgTouchDetector.RectObserveAction -= OnWallUrgTouchDetectorRectObserveAction;
    }

    private void OnWallUrgTouchDetectorRectObserveAction(string arg1, UrgGridObserverData arg2)
    {
        //left
        //right
        switch (arg1)
        {
            case "left":
                Debug.Log($"[LEFT]: {arg2.averageSum}");
                break;
            case "right":
                Debug.Log($"[RIGHT]: {arg2.averageSum}");
                break;
        }
    }

    private void LateUpdate()
    {
        //FloorUpdate();
    }


    #region FloorUrgDetector
private void FloorUpdate()
    {
        if (FloorTouchCount > 0)
        {
            _floorInvokeTime += Time.deltaTime;
            if (_floorInvokeTime > floorInvokeTimeMax)
            {
                _floorEmptyTime = 0;
                _isSomeone = true;
            }
        }

        if (FloorTouchCount == 0)
        {
            _floorEmptyTime += Time.deltaTime;
            if (_floorEmptyTime > floorEmptyTimeMax)
            {
                _floorInvokeTime = 0;
                _isSomeone = false;
            }
        }
        
        FloorStepUpdate();
    }

    private void FloorStepUpdate()
    {
        if (IsSomeone)
        {
            var allTouch = Managers.Game.FloorUrgTouchDetector.AllScreenTouchList;
        
        
            while (_stepList.Count < allTouch.Count)
            {
                var step = Managers.Resource.Instantiate("Floor/Step");
                _stepList.Add(step);
            }

            if (_stepList.Count > allTouch.Count)
            {
                for (int i = 0; i < _stepList.Count; i++)
                {
                    if (i > allTouch.Count - 1)
                    {
                        Managers.Resource.Destroy(_stepList[i]);
                        _stepList.RemoveAt(i);
                    }
                }
            }

            for (int i = 0; i < allTouch.Count; i++)
            {
                var state = allTouch[i].touchState;
                _stepList[i].gameObject.SetActive(state == UrgTouchState.TouchPress);
                
                switch (state)
                {
                    case UrgTouchState.Empty:
                        break;
                    case UrgTouchState.TouchMoment:
                        break;
                    case UrgTouchState.TouchDown:
                        break;
                    case UrgTouchState.TouchPress:
                        var pos = _stepList[i].transform.position;
                        var getPos = (Vector2)Managers.Game.FloorCamera.ViewportToWorldPoint(allTouch[i].viewPortPos);
                        _stepList[i].transform.position = new Vector3(getPos.x, getPos.y, pos.z);   
                        break;
                    case UrgTouchState.TouchPressUp:
                        break;
                    case UrgTouchState.TouchClicked:
                        break;
                }
            }
        }
        else
        {
            for (int i = 0; i < _stepList.Count; i++)
            {
                Managers.Resource.Destroy(_stepList[i]);
                _stepList.RemoveAt(i);
            }
        }
    }
    #endregion
    
}
