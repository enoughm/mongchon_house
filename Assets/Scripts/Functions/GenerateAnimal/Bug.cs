using System;
using Spine.Unity;
using Spine.Unity.Examples;
using SWS;
using UnityEngine;
using UnityEngine.AI;

public class Bug : MonoBehaviour
{
    public enum State
    {
        None,
        Entering,
        InSeat,
        OutSeat,
        RandomMove,
        Step,
        Exiting,
        Die,
    }

    [Header("Components")]
    [SerializeField] Transform randomMoveCenter;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] SkeletonAnimationHandleExample seatAnimationHandler;
    [SerializeField] GameObject dustPrefab;

    [Header("Options")] 
    public float enterSpeed = 7f;
    public float outSeatSpeed = 5;
    public float randomMoveRange = 8;
    public float randomMoveDuration = 60f;
    
    [Header("Update Values (일회성)")]
    public bool isStep = false;
    
    [Header("Event")]
    public Action OnDie;
    
    private splineMove _splineMove;
    private NavMeshAgent _navMeshAgent;
    private SeatController _curSeatController;

    private float _randomMoveTime;
    private State _prevState, _curState;
    private Camera _targetCamera;

    
    private void Awake()
    {
        _splineMove = GetComponent<splineMove>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _targetCamera = GameObject.FindGameObjectWithTag("FloorCam").GetComponent<Camera>();
    }

    private void OnEnable()
    {
        Managers.Input.MouseAction += OnMouseEvent;
    }

    private void OnDisable()
    {
        Managers.Input.MouseAction -= OnMouseEvent;
    }

    private void Start()
    {
        _prevState = State.None;
        _curState = State.None;
    }

    private void Update()
    {
        if (_prevState != _curState)
        {
            _prevState = _curState;
            Debug.Log($"State Changed : from [{_prevState}] to [{_curState}]");
            switch (_curState)
            {
                case State.None:
                    break;
                case State.Entering:
                    break;
                case State.InSeat:
                    break;
                case State.OutSeat:
                    EnterState_OutSeat();
                    break;
                case State.RandomMove:
                    EnterState_RandomMove();
                    break;
                case State.Step:
                    break;
                case State.Exiting:
                    break;
                case State.Die:
                    EnterState_Die();
                    break;
            }
        }

        
        switch (_curState)
        {
            case State.None:
                //nothing
                break;
            case State.Entering:
                break;
            case State.InSeat:
                break;
            case State.OutSeat:
                break;
            case State.RandomMove:
                Update_RandomMove();
                break;
            case State.Step:
                break;
            case State.Exiting:
                break;
            case State.Die:
                //Debug.Log("Die");
                break;
        }

        isStep = false;
    }

    public void SetState(State state)
    {
        _curState = state;
        Debug.Log($"SetState: [{_prevState}] to [{_curState}]");
    }
    
    /// <summary>
    /// 방석밑으로 들어갑니다
    /// </summary>
    /// <param name="seat"></param>
    public void EnterToSeat(SeatController seat, int priority)
    {
        SetState(State.Entering);
        
        _curSeatController = seat;
        _splineMove.ChangeSpeed(enterSpeed);
        _navMeshAgent.avoidancePriority = priority;

        SplineMoveOn();
        PlayAnim();

        _splineMove.pathContainer = seat.EnterPath;
        _splineMove.loopType = splineMove.LoopType.none;
        _splineMove.StartMove();

        _splineMove.movementEnd.RemoveAllListeners();
        _splineMove.movementEnd.AddListener(() =>
        {
            _curSeatController.InsertBug(this);
            SetState(State.InSeat);
        });
    }

    #region State_OutSeat
    private void EnterState_OutSeat()
    {
        SplineMoveOn();
        
        _splineMove.pathContainer = _curSeatController.GetRandomExitPath();
        _splineMove.speed = outSeatSpeed;
        _splineMove.loopType = splineMove.LoopType.none;
        _splineMove.movementChange.RemoveAllListeners();
        _splineMove.movementEnd.RemoveAllListeners();
        _splineMove.movementEnd.AddListener(() =>
        {
            SetState(State.RandomMove);
        });
        
        _splineMove.StartMove();
    }

    #endregion
    

    #region State_RandomMove
    private void EnterState_RandomMove()
    {
        //랜덤 이동 시간 초기화
        _randomMoveTime = 0;
        //SplineMoveOff
        SplineMoveOff();
    }
    
    private void Update_RandomMove()
    {
        if (!_navMeshAgent.hasPath)
        {
            Vector3 point;
            if (RandomPoint(randomMoveCenter.position, randomMoveRange, out point))
            {
                Debug.Log(point);
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                _navMeshAgent.SetDestination(point);
            }
        }
        
        
        if(_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) 
        {
            Vector3 point;
            if (RandomPoint(randomMoveCenter.position, randomMoveRange, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                _navMeshAgent.SetDestination(point);
            }
        }

        if (isStep)
        {
            Debug.Log("밟힘}");
            SetState(State.Die);
            return;
        }
            
        _randomMoveTime += Time.deltaTime;
        if (_randomMoveTime > randomMoveDuration)
        {
            SetState(State.Die);
            return;
        }
    }
    #endregion

    #region State_Die
    private void EnterState_Die()
    {
        SplineMoveOff();
        var obj = Managers.Resource.Instantiate("Effect/Dust");
        obj.transform.position = transform.position;
        obj.transform.rotation = Quaternion.identity;
        OnDie?.Invoke();
        Destroy(this.gameObject, 0.3f);
    }
    #endregion


    #region Functions
    private void SplineMoveOff()
    {
        _navMeshAgent.enabled = true;
        if(_splineMove.enabled)
            _splineMove.Stop();
        _splineMove.enabled = false;
    }

    private void SplineMoveOn()
    {
        _navMeshAgent.enabled = false;
        _splineMove.enabled = true;
    }
    
    private void SplineAndRandomMoveOff()
    {
        _navMeshAgent.enabled = false;
        if(_splineMove.enabled)
            _splineMove.Stop();
        _splineMove.enabled = false;
    }
    
    
    /// <summary>
    /// 움직임 애니메이션
    /// </summary>
    private void PlayAnim()
    {
        seatAnimationHandler.PlayAnimationForState("run", 0);
    }

    
    /// <summary>
    /// To Default Animation
    /// </summary>
    private void StopAnim()
    {
        skeletonAnimation.ClearState();
    }
    #endregion



    #region Utils
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
    #endregion




    #region Event
    private void OnMouseEvent(Define.MouseEvent obj)
    {
        if (obj != Define.MouseEvent.Click)
            return;

        //raycast가 chractercontroller 입력 받았는지 확인
        Ray ray = _targetCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Bug");
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            Debug.Log(hit.transform.gameObject.name);
            if (hit.transform.gameObject.Equals(gameObject))
            {
                Debug.Log("Bug Clicked");
                isStep = true;
            }
        }
    }
    #endregion
}
