using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Spine.Unity;
using Spine.Unity.Examples;
using SWS;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class Bug : MonoBehaviour
{
    public bool isSeated = false;
    public bool isSteped = false;
    
    
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] SkeletonAnimationHandleExample seatAnimationHandler;
    [SerializeField] PathManager randomPaths;
    [SerializeField] List<PathManager> exitPaths = new List<PathManager>();
    [SerializeField] GameObject dustPrefab;
    
    navMove navMove; //nav용
    splineMove splineMove; //enter용
    NavMeshAgent _navMeshAgent;
    ExitMovement _exitMovement;

    SeatController _curSeatController;

    private bool isNavMove = false;
    private float navMoveTime = 0;
    private float navMoveDuration = 15f;

    public Action onDie;

    private bool yoyoCheck = false;

    private void Awake()
    {
        navMove = GetComponent<navMove>();
        splineMove = GetComponent<splineMove>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _exitMovement = GetComponent<ExitMovement>();
    }

    private void OnEnable()
    {
        Managers.Input.MouseAction += OnMouseEvent;
    }

    private void OnDisable()
    {
        Managers.Input.MouseAction -= OnMouseEvent;
    }

    private void OnMouseEvent(Define.MouseEvent obj)
    {
        if (obj != Define.MouseEvent.Click)
            return;

        
        Debug.Log("OnMouseEvent4");
        //raycast가 chractercontroller 입력 받았는지 확인
        Ray ray = GameObject.Find("Floor Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Bug");
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.transform.gameObject.name == gameObject.name)
            {
                Debug.Log("Bug Clicked");
                isSteped = true;
            }
        }
    }
    
    private void Update()
    {
        if (isNavMove)
        {
            if (isSteped)
            {
                Die();
                return;
            }
            
            navMoveTime += Time.deltaTime;
            if (navMoveTime > navMoveDuration)
            {
                SetMoveStyleExit();
                navMove.SetPath(exitPaths[UnityEngine.Random.Range(0, exitPaths.Count)]);
                navMove.movementEnd.RemoveAllListeners();
                navMove.ChangeSpeed(6);
                navMove.movementEnd.AddListener(Die);
                navMoveTime = 0;
                isNavMove = false;
            }
        }

        isSteped = false;
    }

    public void Die()
    {
        isSteped = false;
        isNavMove = false;
        navMoveTime = 0;
        //die
        var dust = Instantiate(dustPrefab, transform.position, Quaternion.identity);
        onDie?.Invoke();
        SetMoveStyleStop();
        Destroy(this.gameObject, 0.3f);
    }

    private void SetMoveStyleToNav()
    {
        navMove.loopType = navMove.LoopType.random;
        _navMeshAgent.enabled = true;
        navMove.enabled = true;
        splineMove.enabled = false;
    }

    private void SetMoveStyleToSpline()
    {
        isSeated = false;
        _navMeshAgent.enabled = false;
        navMove.enabled = false;
        splineMove.enabled = true;
    }

    private void SetMoveStyleExit()
    {
        //StopAnim();
        _navMeshAgent.enabled = true;
        if(navMove.enabled)
            navMove.Stop();
        if(splineMove.enabled)
            splineMove.Stop();
        navMove.enabled = false;
        splineMove.enabled = false;
    }
    
    private void SetMoveStyleStop()
    {
        //StopAnim();
        _navMeshAgent.enabled = false;
        if(navMove.enabled)
            navMove.Stop();
        if(splineMove.enabled)
            splineMove.Stop();
        navMove.enabled = false;
        splineMove.enabled = false;
    }
    
    /// <summary>
    /// 방석밑으로 들어갑니다
    /// </summary>
    /// <param name="seat"></param>
    public bool EnterToSeat(SeatController seat, float speed, int priority)
    {
        if (!seat.CanEnter)
            return false;

        SetMoveStyleToSpline();
        PlayAnim();

        _curSeatController = seat;
        splineMove.pathContainer = seat.EnterPath;
        splineMove.ChangeSpeed(speed);
        splineMove.loopType = splineMove.LoopType.none;
        splineMove.StartMove();
        _navMeshAgent.avoidancePriority = priority;
        splineMove.movementEnd.RemoveAllListeners();
        splineMove.movementEnd.AddListener(EnterToSeatComplete);
        seat.InsertBug(this);
        return true;
    }
    
    private void EnterToSeatComplete()
    {
        _curSeatController.PlayAnim();
        isSeated = true;
    }

    public bool ShowingTemp(PathManager targetPath, float speed)
    {
        if (!isSeated)
            return false;
        
        SetMoveStyleToSpline();

        splineMove.pathContainer = targetPath;
        splineMove.speed = speed;
        splineMove.loopType = splineMove.LoopType.yoyo;
        splineMove.StartMove();
        splineMove.movementChange.RemoveAllListeners();
        splineMove.movementChange.AddListener(OnMovementChange);
        splineMove.movementEnd.RemoveAllListeners();
        splineMove.movementEnd.AddListener(ShowingTempEnd);
        return true;
    }

    private void ShowingTempEnd()
    {
        yoyoCheck = true; 
    }

    private void OnMovementChange(int arg0)
    {
        if (yoyoCheck)
        {
            splineMove.Stop();
            yoyoCheck = false;
        }
        
        if(arg0 != 0)
            splineMove.Pause(0.25f);
    }

    public void RunFromSeat(PathManager targetPath, float speed)
    {
        if (!isSeated)
            return;

        SetMoveStyleToSpline();

        splineMove.pathContainer = targetPath;
        splineMove.speed = speed;
        splineMove.loopType = splineMove.LoopType.none;
        splineMove.StartMove();
        splineMove.movementChange.RemoveAllListeners();
        splineMove.movementEnd.RemoveAllListeners();
        splineMove.movementEnd.AddListener(RunFinish);
    }

    private void RunFinish()
    {
        SetMoveStyleToNav();
        isNavMove = true;
        navMoveTime = 0;
        navMove.StartMove();
    }


    /// <summary>
    /// 움직임 애니메이션
    /// </summary>
    public void PlayAnim()
    {
        seatAnimationHandler.PlayAnimationForState("run", 0);
    }
    
    
    /// <summary>
    /// To Default Animation
    /// </summary>
    public void StopAnim()
    {
        //skeletonAnimation.AnimationState.;
    }
}
