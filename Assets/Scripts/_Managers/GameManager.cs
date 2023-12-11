using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string gameState;
    public TCPServerManager Server
    {
        get
        {
            if(_server == null)
                _server = FindObjectOfType<TCPServerManager>();
            return _server;
        }
    }
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
    private TCPServerManager _server;

    private bool _isSomeone = false;
    private float _floorInvokeTime = 0;
    private float _floorEmptyTime = 0;
    private Camera _floorCamera;
    private List<GameObject> _stepList = new List<GameObject>();

    
    private Camera _wallCamera;

    
    private void Awake()
    {
        _wallUrgTouchDetector = GameObject.FindGameObjectWithTag("WallUrgTouchDetector").GetComponent<UrgTouchDetector>();
        _wallUrgTouchDetector.HokuyoAction += OnFloorAction;
    }
    
    private void OnEnable()
    {
        WallUrgTouchDetector.RectObserveAction += OnWallUrgTouchDetectorRectObserveAction;
        ServerPacketHandler.onPacketSimple += OnPacketSimple;
    }

    private void OnDisable()
    {
        WallUrgTouchDetector.RectObserveAction -= OnWallUrgTouchDetectorRectObserveAction;
        ServerPacketHandler.onPacketSimple -= OnPacketSimple;
    }

    private void Start()
    {
        Managers.Game.ToInitialize();
    }

    private void Update()
    {

    }

    public void SendPacketLightOnLeft()
    {
        TCPServerManager.Room.Broadcast(new PacketSimple()
        {
            packetKey = "LightOnLeft",
            stringData = $"on",
        }.Write());
    }
    
    public void SendPacketLightOnRight()
    {
        TCPServerManager.Room.Broadcast(new PacketSimple()
        {
            packetKey = "LightOnRight",
            stringData = $"on",
        }.Write());
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
   
    
    private void OnPacketSimple(PacketSimple obj)
    {
        Debug.Log($"[Server] OnPacketSample : {obj.stringData}");
        switch (obj.packetKey)
        {
            case "GameState":
                if(gameState == obj.stringData)
                    return;
                
                if (obj.stringData == "NoOne")
                {
                    ToInitialize();
                }
                else
                {
                    Someone();
                }
                
                gameState = obj.stringData;
                break;
        }
    }
    
    private void OnFloorAction(UrgTouchState arg1, Vector2 arg2)
    {
        if (arg1 == UrgTouchState.TouchDown)
        {
            //var obj = Managers.Resource.Instantiate("Particle/벽터치", this.transform, 20);
            //obj.transform.position = FloorCamera.ViewportToWorldPoint(arg2);
        }
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
