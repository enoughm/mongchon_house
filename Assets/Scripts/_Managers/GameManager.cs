using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    public UrgTouchDetector FloorUrgTouchDetector => _floorUrgTouchDetector;
    public List<GameObject> StepList => _stepList;

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
        _wallUrgTouchDetector = GameObject.FindGameObjectWithTag("WallUrgTouchDetector").GetComponent<UrgTouchDetector>();
    }

    private void Start()
    {
        Managers.Game.ToInitialize();
    }

    private void Update()
    {

    }

    [Button]
    public void ToInitialize()
    {
        FindObjectOfType<BabyStateMachine>().TurnOffLight();
    }

    [Button]
    public void Someone()
    {
        FindObjectOfType<BabyStateMachine>().TurnOnLight();
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
}
