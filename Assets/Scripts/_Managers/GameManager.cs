using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityHFSM;

public class GameManager : MonoBehaviour
{
    public TCPClientManager Packet
    {
        get
        {
            if(_client == null)
                _client = FindObjectOfType<TCPClientManager>();
            return _client;
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
    public bool IsSomeone => _isSomeone;
    public int FloorTouchCount => _floorUrgTouchDetector.AllScreenTouchList.Count;
    public UrgTouchDetector FloorUrgTouchDetector => _floorUrgTouchDetector;
    public List<GameObject> StepList => _stepList;

    
    
    public float floorEmptyTimeMax = 10;
    public float floorInvokeTimeMax = 3;
    
    
    private bool _isSomeone = false;
    private float _floorInvokeTime = 0;
    private float _floorEmptyTime = 0;
    
    TCPClientManager _client;
    UrgTouchDetector _floorUrgTouchDetector;
    UrgTouchDetector _wallUrgTouchDetector;
    List<GameObject> _stepList = new List<GameObject>();
    Camera _floorCamera;
    StateMachine _stateMachine;
    LightManager _light;
    
    
    /// <summary>
    /// nothingTime은 isSomeone 이 false으로 지속된 시간을 의미
    /// </summary>
    private float nothingTime = 0;
    private float nothingMaxTime = 15;
    
    

    
    private void Awake()
    {
        _floorUrgTouchDetector = GameObject.FindGameObjectWithTag("FloorUrgTouchDetector").GetComponent<UrgTouchDetector>();
        _client = FindObjectOfType<TCPClientManager>();
        _light = FindObjectOfType<LightManager>();
    }

    private void OnEnable()
    {
        ClientPacketHandler.OnPacketSimple += OnPacketSimple;
    }
    
    private void OnDisable()
    {
        ClientPacketHandler.OnPacketSimple -= OnPacketSimple;
    }


    private void Start()
    {
        _stateMachine = new StateMachine();
        _stateMachine.AddState("NoOne", 
            new State(
                onEnter: _ =>
                {
                    Debug.Log("@@@@@@@@@@@@@@ NO ONE ENTER");
                    _light.ToDark(3);
                    Managers.Sound.PlayMusic(Track.LightOffMusic);
                },
                onLogic: _ =>
                {
                    
                }, 
                onExit: _ =>
                {
                    
                }));
        
        _stateMachine.AddState("Anyone", 
            new State(
                onEnter: _ =>
                {
                    Managers.Sound.StopMusic();
                    DOVirtual.DelayedCall(1.5f, () =>
                    {
                        Managers.Sound.PlayMusic(Track.LightOnMusic);
                    });
                    nothingMaxTime = SROptions.Current.WaitTIme;
                },
                onLogic: _ =>
                {
                    
                }, 
                onExit: _ =>
                {
                    
                }));
       
       
        _stateMachine.SetStartState("NoOne");
        _stateMachine.AddTransition(new Transition(
            "NoOne",
            "Anyone",
            transition =>
            {
                return IsSomeone;
            }
        ));
        
        _stateMachine.AddTransition(new Transition(
            "Anyone", 
            "NoOne", 
            transition =>
            {
                return nothingTime > nothingMaxTime;
            }
        ));
        _stateMachine.Init();
        StartCoroutine(CoUpdate());
    }

    private void Update()
    {
        if (!IsSomeone)
            nothingTime += Time.deltaTime;
        else
            nothingTime = 0;
        _stateMachine.OnLogic();
        
        
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            SRDebug.Instance.PinAllOptions("Audio");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            SRDebug.Instance.PinAllOptions("Playing");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            SRDebug.Instance.ClearPinnedOptions();
        }
    }

    private void LateUpdate()
    {
        FloorUpdate();
    }

    IEnumerator CoUpdate()
    {
        yield return null;
        if(_client == null)
            _client = FindObjectOfType<TCPClientManager>();
        while (true)
        {
            Packet.Send(new PacketSimple()
            {
                packetKey = "GameState",
                stringData = _stateMachine.ActiveStateName,
            }.Write());
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    
    private void OnPacketSimple(PacketSimple obj)
    {
        Debug.Log($"[Client] OnPacketSample : {obj.stringData}");
       
        switch (obj.packetKey)
        {
            case "LightOnLeft":
                _light.ToLightLeft();
                break;
            case "LightOnRight":
                _light.ToLightRight();
                break;
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
