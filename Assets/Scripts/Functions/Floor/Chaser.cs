using System;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Chaser : MonoBehaviour
{
    public enum State
    {
        None,
        RandomMove,
        Chasing,
        Waiting,
        Avoiding,
    }
    
    /// <summary>
    /// 순서
    /// 아무도 없을 때, 천천히 random move
    /// 누군가 있을 때, Waiting
    /// 제일 가까은 쪽으로, Chasing
    /// 만약 밟을 경우, Avoiding
    /// 피하고 난 후 Waiting
    /// 아무도 없을 때, random move
    /// </summary>
    
    [Header("Update Values (일회성)")]
    public bool isStep = false;
    private bool isSomeone => Managers.Game.IsSomeone;

    [Header("Components")]
    [SerializeField] Transform randomMoveCenter;
    [SerializeField] Transform avoidingCenter;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    
    [Header("Options")]
    public float avoidingSpeed = 10f;
    public float avoidingRange = 0.25f;
    public float avoidingAcceleration = 10;
    public float avoidingAnimTimeScale = 2.5f;
    
    [FormerlySerializedAs("chasingSuccess")] public bool chasingCacluationSuccess = false;
    public float chasingSpeed = 7f;
    public float chasingAcceleration = 3;
    public float chasingTimeScale = 1.75f;
    public float chasingStopDistance = 1.5f;
    public float chasingMaxDistance = 24f;

    public float waitingSpeed = 0.5f;
    public float waitingRange = 0.5f;
    public float waitingDuration = 2f;
    public float waitingIdleMoveDuration = 1f;
    public float waitingAcceleration = 3;
    public float waitingTimeScale = 0.7f;

    public float randomMoveSpeed = 1.25f;
    public float randomMoveRange = 7f;
    public float randomMoveAcceleration = 3f;
    public float randomMoveTimeScale = 1f;

    private NavMeshAgent _navMeshAgent;
    private State _prevState, _curState;
    private float _waitingTime = 0;
    private float _waitingIdleMoveTime = 0;
    
    private void Awake()
    {
        _prevState = State.None;
        _curState = State.RandomMove;
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Game.FloorUrgTouchDetector.HokuyoAction += OnHokuyoEvent;
    }

    private void OnDisable()
    {
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Game.FloorUrgTouchDetector.HokuyoAction -= OnHokuyoEvent;
    }

    private void Start()
    {
        transform.localScale = Vector3.one * SROptions.Current.ChaserScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (_prevState != _curState)
        {
            Debug.Log($"State Changed : from [{_prevState}] to [{_curState}]");
            _prevState = _curState;
            switch (_curState)
            {
                case State.None:
                    break;
                case State.RandomMove:
                    Enter_RandomMove();
                    break;
                case State.Chasing:
                    Enter_Chasing();
                    break;
                case State.Waiting:
                    Enter_Waiting();
                    break;
                case State.Avoiding:
                    Enter_Avoiding();
                    break;
            }
            return;
        }
        
        switch (_curState)
        {
            case State.None:
                break;
            case State.RandomMove:
                Update_RandomMove();
                break;
            case State.Chasing:
                Update_Chasing();
                break;
            case State.Waiting:
                Update_Waiting();
                break;
            case State.Avoiding:
                Update_Avoiding();
                break;
        }

        isStep = false;
    }
    
    public void SetState(State state, bool force = false)
    {
        transform.localScale = Vector3.one * SROptions.Current.ChaserScale;
        if(force)
            _prevState = State.None;
        _curState = state;
        Debug.Log($"SetState: [{_prevState}] to [{_curState}]");
    }

    #region State_RandomMove
    private void Enter_RandomMove()
    {
        _navMeshAgent.ResetPath();
        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = randomMoveSpeed;
        _navMeshAgent.acceleration = randomMoveAcceleration;
        skeletonAnimation.timeScale = randomMoveTimeScale;
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
            SetState(State.Avoiding);
            return;
        }

        if (isSomeone)
        {
            Debug.Log("누군가 있음");
            SetState(State.Waiting);
        }
    }
    #endregion

    #region State_Avoiding
    private void Enter_Avoiding()
    {
        _navMeshAgent.ResetPath();
        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = avoidingSpeed;
        _navMeshAgent.acceleration = avoidingAcceleration;
        skeletonAnimation.timeScale = avoidingAnimTimeScale;

        if (!_navMeshAgent.hasPath)
        {
            Vector3 point;
            if (RandomPoint(avoidingCenter.position, avoidingRange, out point))
            {
                Debug.Log(point);
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                _navMeshAgent.SetDestination(point);
            }
        }
    }
    
    private void Update_Avoiding()
    {
        if(_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) 
        {
            SetState(State.RandomMove);
            return;
        }

        if (isStep)
        {
            Debug.Log("밟힘}");
            SetState(State.Avoiding, true);
            return;
        }
    }
    #endregion

    #region State_Waiting

    public Transform closeStepInWaiting;
    private void Enter_Waiting()
    {
        _navMeshAgent.ResetPath();
        _navMeshAgent.speed = waitingSpeed;
        _navMeshAgent.acceleration = waitingAcceleration;
        skeletonAnimation.timeScale = waitingTimeScale;
        _waitingTime = 0;
        _waitingIdleMoveTime = 0;
        
        //FindCloseTarget;
        var close = FindCloseStep();
        closeStepInWaiting = close;
        if (closeStepInWaiting != null)
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.SetDestination(close.position);
        }
    }

    private void Update_Waiting()
    {
        _waitingTime += Time.deltaTime;


        closeStepInWaiting = FindCloseStep();
        
        
        // if(Vector2.Distance(this.transform.position, closeStepInWaiting.transform.position) < chasingStopDistance)
        // {
        //     if(_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) 
        //     {
        //         _waitingIdleMoveTime += Time.deltaTime;
        //         if (_waitingIdleMoveTime > waitingIdleMoveDuration)
        //         {
        //             _waitingIdleMoveTime = 0;
        //             Transform tr = closeStepInWaiting == null ? this.transform : closeStepInWaiting;
        //             Vector3 point;
        //             if (RandomPoint(tr.position, waitingRange, out point))
        //             {
        //                 Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
        //                 _navMeshAgent.SetDestination(point);
        //             }
        //         }
        //     }
        //     
        //     return;
        // }
        // if(_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) 
        // {
        //     _waitingIdleMoveTime += Time.deltaTime;
        //     if (_waitingIdleMoveTime > waitingIdleMoveDuration)
        //     {
        //         _waitingIdleMoveTime = 0;
        //         Transform tr = close == null ? this.transform : close;
        //         Vector3 point;
        //         if (RandomPoint(tr.position, waitingRange, out point))
        //         {
        //             Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
        //             _navMeshAgent.SetDestination(point);
        //         }
        //     }
        // }

        

        if (isStep)
        {
            SetState(State.Avoiding);
            return;
        }


        if(_waitingTime > waitingDuration)
        {
            if (closeStepInWaiting == null)
            {
                SetState(State.RandomMove);
                return;
            }
            else
            {
                SetState(State.Chasing);
                return;
            }
        }
    }
    #endregion


    #region State_Chasing
    Vector3 closeStepInChasing;
    private void Enter_Chasing()
    {
        _navMeshAgent.speed = chasingSpeed;
        _navMeshAgent.acceleration = chasingAcceleration;
        skeletonAnimation.timeScale = chasingTimeScale;
        chasingCacluationSuccess = false;

        
        //FindCloseTarget;
        var close = FindCloseStep();
        if (close == null)
        {
            SetState(State.Waiting);
            return;
        }
        
        closeStepInChasing = close.position;
        _navMeshAgent.ResetPath();
        chasingCacluationSuccess = _navMeshAgent.SetDestination(closeStepInChasing);
    }

    private void Update_Chasing()
    { 
        if (!chasingCacluationSuccess)
        {
            SetState(State.Waiting);
            return;
        }

        var dis = Vector2.Distance(this.transform.position, closeStepInChasing);
        
        if(dis < chasingStopDistance)
        {
            _navMeshAgent.ResetPath();
            Debug.Log("Chasing Success");
            SetState(State.Waiting);
            return;
        }

        if (dis > chasingMaxDistance)
        {
            _navMeshAgent.ResetPath();
            Debug.Log("Chasing Success");
            SetState(State.Waiting);
            return;
        }
    }

    #endregion
    
    
    #region Utils
    
    Transform FindCloseStep()
    {
        if (isSomeone)
        {
            var stepList = Managers.Game.StepList;
            float min = float.MaxValue;
            int idx = int.MaxValue;
            for (int i = 0; i < stepList.Count; i++)
            {
                var dist = Vector2.Distance(this.transform.position, stepList[i].transform.position);
                if (dist < min)
                {
                    min = dist;
                    idx = i;
                }
            }

            if (idx < stepList.Count)
                return stepList[idx].transform;
            else
                return null;
        }
        return null;
    }
    
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
        Ray ray = Managers.Game.FloorCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Bug");
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.transform.gameObject.Equals(gameObject))
            {
                Debug.Log("Bug Clicked");
                isStep = true;
            }
        }
    }
    
    private void OnHokuyoEvent(UrgTouchState state, Vector2 arg2)
    {
        switch (state)
        {
            case UrgTouchState.Empty:
                break;
            case UrgTouchState.TouchMoment:
                break;
            case UrgTouchState.TouchDown:
                break;
            case UrgTouchState.TouchPress:
                Ray ray = Managers.Game.FloorCamera.ViewportPointToRay(arg2);
                RaycastHit hit;
                int layerMask = 1 << LayerMask.NameToLayer("Bug");
                if (Physics.Raycast(ray, out hit, 100f, layerMask))
                {
                    if (hit.transform.gameObject.Equals(gameObject))
                    {
                        isStep = true;
                    }
                }
                break;
            case UrgTouchState.TouchPressUp:
                break;
            case UrgTouchState.TouchClicked:
                break;
        }
    }
    #endregion
}
