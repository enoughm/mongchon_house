using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity.Examples;
using UnityEngine;
using UnityHFSM;

public class Baby : MonoBehaviour
{
    public enum State
    {
        None,
        Idle,
        LightOn,
        LightOff,
        MoveToJangnam,
        MoveToChanam,
        MoveToGrandma,
        MoveToChanamWife,
        MoveToIdle,
    }
    public enum AnimationState
    {
        IDLE,
        INTERACTION_ONE,
        INTERACTION_TWO,
        WALK,
        RUN,
        LIGHT_ON
    }
    
    [Header("Move")]
    [SerializeField] Transform moveTarget;
    [SerializeField] Transform lookTarget;
    [SerializeField] float walkSpeed = 2;
    [SerializeField] float runSpeed = 5;
    [SerializeField] float stopDistance = 0.5f;
    [SerializeField] bool isWalk;
    private bool reached;
    
    [Header("Wait")]
    [SerializeField] Transform noneTr; //none tr
    [SerializeField] Transform initTr; //idle tr

    [Header("Components")]
    [SerializeField] LightManager lightManager;
    [SerializeField] CandleController leftCandle;
    [SerializeField] Transform leftCandleFireTr;
    
    [SerializeField] CandleController rightCandle;
    [SerializeField] Transform rightCandleFireTr;
    
    [SerializeField] SkeletonAnimationHandleExample idleObject;
    [SerializeField] SkeletonAnimationHandleExample interactionObject;
    [SerializeField] SkeletonAnimationHandleExample walkAndOnObject;

    [Header("Components_MoveToward")] 
    [SerializeField] private List<Transform> idleToJangnamPathes = new List<Transform>();
    [SerializeField] private List<Transform> jangnamToChanamPathes = new List<Transform>();
    [SerializeField] private List<Transform> chanamToWifePathes = new List<Transform>();
    [SerializeField] private List<Transform> wifeToGrandmaPathes = new List<Transform>();
    [SerializeField] private List<Transform> grandmaToIdlePathes = new List<Transform>();
    
        
        
        
    [SerializeField] Transform idleLeftTr;
    [SerializeField] Transform idleRightTr;
    
    private StateMachine<State> _fsm;
    
    private void Start()
    {
        _fsm = new StateMachine<State>();
        _fsm.AddState(State.None, new CoState<State>(this, None, OnEnter, loop: false));
        _fsm.AddState(State.Idle, new CoState<State>(this, Idle, OnEnter, loop: false));
        _fsm.AddState(State.LightOn, new CoState<State>(this, LightOn, OnEnter, loop: false));
        _fsm.AddState(State.LightOff, new CoState<State>(this, LightOff, OnEnter, loop: false));
        _fsm.AddState(State.MoveToJangnam, new CoState<State>(this, MoveToJangnam, OnEnter, loop: false));
        _fsm.AddState(State.MoveToChanam, new CoState<State>(this, MoveToChanam, OnEnter, loop: false));
        _fsm.AddState(State.MoveToGrandma, new CoState<State>(this, MoveToGrandma, OnEnter, loop: false));
        _fsm.AddState(State.MoveToChanamWife, new CoState<State>(this, MoveToChanamWife, OnEnter, loop: false));
        _fsm.AddState(State.MoveToIdle, new CoState<State>(this, MoveToIdle, OnEnter, loop: false));
        _fsm.SetStartState(State.None);
        _fsm.Init();
        
        lightManager.ToDark(3);
        PlayAnimation(AnimationState.INTERACTION_ONE);
    }

    private void OnEnter(CoState<State, string> obj)
    {
    }

    public void TurnOnLight()
    {
        StopAllCoroutines();
        _fsm.RequestStateChange(State.LightOn);
    }

    public void TurnOffLight()
    {
        StopAllCoroutines();
        _fsm.RequestStateChange(State.LightOff);
    }

    private void Update()
    {
        MoveToTargetTr();
        _fsm.OnLogic();
    }

    #region Func
    private void SetRight()
    {
        transform.localScale = new Vector3(-1, 1, 1);
    }

    private void SetLeft()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    private void SetMoveTarget(Transform transform, bool isWalk)
    {
        this.isWalk = isWalk;
        reached = false;
        moveTarget = transform;
    }
    
    private void MoveToTargetTr()
    {
        if (moveTarget == null)
            return;

        if (Vector3.Distance(transform.position , moveTarget.position) < stopDistance )
        {
            reached = true;
            return;
        }
        else
        {
            reached = false;
        }
        
        var speed = isWalk ? walkSpeed : runSpeed;
        //vector move
        Vector3 dir = moveTarget.position - transform.position;
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
    }
    #endregion

    private IEnumerator None(CoState<State, string> arg)
    {
        //throw new NotImplementedException();
        //251.1 199.37 (3)
        //(7)
        yield break;
    }
    
    //불을 키고나서 좌우로 몇번 느리게 왔다갔다 걸어다닌 후에
    //장남에게 이동한다.
    IEnumerator Idle()
    {
        //좌우로 왔다갔다 무한 반복
        PlayAnimation(AnimationState.WALK);
        int count = 0;
        
        
        SetRight();
        SetMoveTarget(idleRightTr, true);
        while (!reached)
        {
            yield return null;
        }
            
        SetLeft();
        SetMoveTarget(idleLeftTr, true);
        while (!reached)
        {
            yield return null;
        }
        
        _fsm.RequestStateChange(State.MoveToJangnam);
    }
    
    //init Position에서 불을 키는 애니메이션을 시작한다.
    IEnumerator LightOn()
    {
        SetLeft();
        PlayAnimation(AnimationState.RUN);
        SetMoveTarget(rightCandleFireTr, false);
        while (!reached)
        {
            yield return null;
        }
        
        SetLeft();
        var anim1 = PlayAnimation(AnimationState.LIGHT_ON);
        rightCandle.LightOn(1, null);
        Managers.Game.SendPacketLightOnRight();
        yield return new WaitForSeconds(anim1.GetAnimationForState("on").Duration);
        
        
        SetMoveTarget(leftCandleFireTr, false);
        PlayAnimation(AnimationState.RUN);
        while (!reached)
        {
            yield return null;
        }
        SetLeft();
        PlayAnimation(AnimationState.LIGHT_ON);
        leftCandle.LightOn(1, null);
        Managers.Game.SendPacketLightOnLeft();
        yield return new WaitForSeconds(anim1.GetAnimationForState("on").Duration);

        lightManager.ToLight(3);
        SetMoveTarget(initTr, false);
        PlayAnimation(AnimationState.RUN);
        SetRight();
        while (!reached)
        {
            yield return null;
        }

        _fsm.RequestStateChange(State.Idle);
    }
    
    
    //불을 다 끄고 initPosition으로 간 후 초기상태가 된다.
    IEnumerator LightOff()
    {
        leftCandle.LightOff();
        rightCandle.LightOff();
        lightManager.ToDark(3);
        
        SetMoveTarget(noneTr, false);
        PlayAnimation(AnimationState.RUN);
        if(noneTr.transform.position.x < transform.position.x)
            SetLeft();
        else
            SetRight();
        
        while (!reached)
        {
            yield return null;
        }

        yield return null;
        PlayAnimation(AnimationState.INTERACTION_ONE);
        _fsm.RequestStateChange(State.None);
    }
    
    //차남한테 이동한다
    IEnumerator MoveToJangnam()
    {
        for (int i = 0; i < idleToJangnamPathes.Count; i++)
        {
            SetLeft();
            if (i == 0 || i == idleToJangnamPathes.Count - 1)
            {
                PlayAnimation(AnimationState.WALK);
                SetMoveTarget(idleToJangnamPathes[i], true);
            }
            else
            {
                PlayAnimation(AnimationState.RUN);
                SetMoveTarget(idleToJangnamPathes[i], false);
            }
            
            while (!reached)
            {
                yield return null;
            }
        }
        SetMoveTarget(null, false);
        var handle = PlayAnimation(AnimationState.INTERACTION_ONE);
        var wait = handle.GetAnimationForState("interaction").Duration;
        yield return new WaitForSeconds(wait);
        
        PlayAnimation(AnimationState.INTERACTION_TWO);
        var wait2 = handle.GetAnimationForState("interaction2").Duration;
        yield return new WaitForSeconds(wait2);
        
        _fsm.RequestStateChange(State.MoveToChanam);
    }
    
    //차남한테 차남 와이프한테 이동한다
    IEnumerator MoveToChanam()
    {
        for (int i = 0; i < jangnamToChanamPathes.Count; i++)
        {
            SetLeft();
            if (i == 0 || i == jangnamToChanamPathes.Count - 1)
            {
                PlayAnimation(AnimationState.WALK);
                SetMoveTarget(jangnamToChanamPathes[i], true);
            }
            else
            {
                PlayAnimation(AnimationState.RUN);
                SetMoveTarget(jangnamToChanamPathes[i], false);
            }
            
            while (!reached)
            {
                yield return null;
            }
        }
        SetMoveTarget(null, false);
        var handle = PlayAnimation(AnimationState.INTERACTION_ONE);
        var wait = handle.GetAnimationForState("interaction").Duration;
        yield return new WaitForSeconds(wait);
        
        PlayAnimation(AnimationState.INTERACTION_TWO);
        var wait2 = handle.GetAnimationForState("interaction2").Duration;
        yield return new WaitForSeconds(wait2);
        
        _fsm.RequestStateChange(State.MoveToChanamWife);
    }
    
    //할머니한테 이동한다
    IEnumerator MoveToChanamWife()
    {
        for (int i = 0; i < chanamToWifePathes.Count; i++)
        {
            SetRight();
            if (i == 0 || i == chanamToWifePathes.Count - 1)
            {
                PlayAnimation(AnimationState.WALK);
                SetMoveTarget(chanamToWifePathes[i], true);
            }
            else
            {
                PlayAnimation(AnimationState.RUN);
                SetMoveTarget(chanamToWifePathes[i], false);
            }
            
            while (!reached)
            {
                yield return null;
            }
        }
        
        SetMoveTarget(null, true);
        var handle = PlayAnimation(AnimationState.INTERACTION_ONE);
        var wait = handle.GetAnimationForState("interaction").Duration;
        yield return new WaitForSeconds(wait);
        
        PlayAnimation(AnimationState.INTERACTION_TWO);
        var wait2 = handle.GetAnimationForState("interaction2").Duration;
        yield return new WaitForSeconds(wait2);
        
        _fsm.RequestStateChange(State.MoveToGrandma);
    }
    
    //Idle 상태로 이동한다
    IEnumerator MoveToGrandma()
    {
        for (int i = 0; i < wifeToGrandmaPathes.Count; i++)
        {
            SetRight();
            if (i == 0 || i == wifeToGrandmaPathes.Count - 1)
            {
                PlayAnimation(AnimationState.WALK);
                SetMoveTarget(wifeToGrandmaPathes[i], true);
            }
            else
            {
                PlayAnimation(AnimationState.RUN);
                SetMoveTarget(wifeToGrandmaPathes[i], false);
            }

            while (!reached)
            {
                yield return null;
            }
        }
        SetMoveTarget(null, true);
        var handle = PlayAnimation(AnimationState.INTERACTION_ONE);
        var wait = handle.GetAnimationForState("interaction").Duration;
        yield return new WaitForSeconds(wait);
        
        PlayAnimation(AnimationState.INTERACTION_TWO);
        var wait2 = handle.GetAnimationForState("interaction2").Duration;
        yield return new WaitForSeconds(wait2);
        
        _fsm.RequestStateChange(State.MoveToIdle);

    }
    
    //Idle 상태로 이동한다
    IEnumerator MoveToIdle()
    {
        for (int i = 0; i < grandmaToIdlePathes.Count; i++)
        {
            SetLeft();
            if (i == 0 || i == grandmaToIdlePathes.Count - 1)
            {
                PlayAnimation(AnimationState.WALK);
                SetMoveTarget(grandmaToIdlePathes[i], true);
            }
            else
            {
                PlayAnimation(AnimationState.RUN);
                SetMoveTarget(grandmaToIdlePathes[i], false);
            }

            while (!reached)
            {
                yield return null;
            }
        }
        SetMoveTarget(null, true);
        var handle = PlayAnimation(AnimationState.INTERACTION_ONE);
        var wait = handle.GetAnimationForState("interaction").Duration;
        yield return new WaitForSeconds(wait);
        
        PlayAnimation(AnimationState.INTERACTION_TWO);
        var wait2 = handle.GetAnimationForState("interaction2").Duration;
        yield return new WaitForSeconds(wait2);
        
        _fsm.RequestStateChange(State.Idle);
    }

    
    
    SkeletonAnimationHandleExample PlayAnimation(AnimationState state)
    {
        var handle = UpdateObjects(state);
        switch (state)
        {
            case AnimationState.IDLE:
                handle.skeletonAnimation.loop = true;
                break;
            case AnimationState.INTERACTION_ONE:
                handle.skeletonAnimation.loop = true;
                handle.PlayAnimationForState("interaction", 0);
                break;
            case AnimationState.INTERACTION_TWO:
                handle.skeletonAnimation.loop = true;
                handle.PlayAnimationForState("interaction2", 0);
                break;
            case AnimationState.WALK:
                handle.skeletonAnimation.loop = true;
                handle.PlayAnimationForState("walk", 0);
                break;
            case AnimationState.RUN:
                handle.skeletonAnimation.loop = true;
                handle.PlayAnimationForState("run", 0);
                break;
            case AnimationState.LIGHT_ON:
                handle.skeletonAnimation.loop = false;
                handle.PlayAnimationForState("on", 0);
                break;
        }

        return handle;
    }
    
    private SkeletonAnimationHandleExample UpdateObjects(AnimationState state)
    {
        idleObject.gameObject.SetActive(state == AnimationState.IDLE);
        interactionObject.gameObject.SetActive(state is AnimationState.INTERACTION_ONE or AnimationState.INTERACTION_TWO);
        walkAndOnObject.gameObject.SetActive(state is AnimationState.LIGHT_ON or AnimationState.WALK or AnimationState.RUN);

        switch (state)
        {
            case AnimationState.IDLE:
                return idleObject;
            case AnimationState.INTERACTION_ONE:
            case AnimationState.INTERACTION_TWO:
                return interactionObject;
            case AnimationState.WALK:
            case AnimationState.RUN:
            case AnimationState.LIGHT_ON:
                return walkAndOnObject;
        }
        return idleObject;
    }
}
