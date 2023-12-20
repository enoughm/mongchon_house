using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string gameState;
    public AreaDetector BowlDetector => _bowlDetector;
    public AreaDetector PlateDetector => _plateDetector;
    public TCPServerManager Server
    {
        get
        {
            if(_server == null)
                _server = FindObjectOfType<TCPServerManager>();
            return _server;
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


    UrgTouchDetector _wallUrgTouchDetector;
    private TCPServerManager _server;
    private AreaDetector _bowlDetector;
    private AreaDetector _plateDetector;
    private Camera _wallCamera;

    public Action<bool> onLightStateChanged;

    
    private void Awake()
    {
        _wallUrgTouchDetector = GameObject.FindGameObjectWithTag("WallUrgTouchDetector").GetComponent<UrgTouchDetector>();
        _wallUrgTouchDetector.HokuyoAction += OnFloorAction;
        
        _wallUrgTouchDetector.TryGetDetector("bowl", out _bowlDetector);
        _wallUrgTouchDetector.TryGetDetector("plate", out _plateDetector);

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
        if (Input.GetKeyDown(KeyCode.I))
        {
            SRDebug.Instance.PinAllOptions("Seats");
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            SRDebug.Instance.PinAllOptions("Audio");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SRDebug.Instance.ClearPinnedOptions();
        }
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
        FindObjectOfType<Baby>().TurnOffLight();
        onLightStateChanged?.Invoke(false);
    }

    [Button]
    public void Someone()
    {
        FindObjectOfType<Baby>().TurnOnLight();
        onLightStateChanged?.Invoke(true);
    }
   
    
    private void OnPacketSimple(PacketSimple obj)
    {
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
