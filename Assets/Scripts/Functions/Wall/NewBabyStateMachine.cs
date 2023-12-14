using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityHFSM;

public class NewBabyStateMachine : MonoBehaviour
{
    public enum State
    {
        //켜지기전 대기상태
        Wait,
        //불을 키는 상태
        LightOn,
        //제 자리에 가만히 있는 상태
        StateStayInPlaceForRandomMove,
        //앞으로 나와서 이야기하는 상태
        MoveForward,
        MoveBackward,
        //차남 옆에가서 인터렉션하는 상태
        MoveToChanam,
        //장남 옆에가서 인터렉션하는 상태
        MoveToJangnam,
        //할머니 옆에가서 인터렉션하는 상태
        MoveToGrandma,
        //차남며느리 옆에가서 인터렉션하는 상태
        MoveToChanamWife,
    }
    
    
    [Header("Move")]
    [SerializeField] float speed = 6;
    [SerializeField] private float moveStopDistance = 0.05f;

    
    [Header("Wait")]
    [SerializeField] Transform waitTr;
    
    
    [Header("Components")]
    [SerializeField] LightManager lightManager;
    [SerializeField] BabyAnimStateMachine animationMachine;
    [SerializeField] CandleController leftCandle;
    [SerializeField] Transform leftCandleFireTr;
    
    [SerializeField] CandleController rightCandle;
    [SerializeField] Transform rightCandleFireTr;
    
    [Header("Components_MoveToward")]
    [SerializeField] Transform chanamTr;
    [SerializeField] Transform chanamSeatTr;
    [SerializeField] Transform jangnamTr;
    [SerializeField] Transform jangnamSeatTr;
    [SerializeField] Transform grandmaTr;
    [SerializeField] Transform grandmaSeatTr;
    [SerializeField] Transform chanamWifeTr;
    [SerializeField] Transform chanamWifeSeatTr;
    
    
    [SerializeField] Vector3 moveForwardStartPos;
    [SerializeField] Transform moveForwardYTr;
    
    
    
    private StateMachine<State> _fsm;
    
    
    void Start()
    {
        _fsm = new StateMachine<State>();

        _fsm.AddState(State.Wait, onEnter: _ => {
            Debug.Log(" [Enter WaitState]");
            lightManager.ToDark(3);
            StopAllCoroutines();
            MoveToTargetTr(waitTr, null, null);
        });
        _fsm.AddState(State.LightOn, new CoState<State>(this, StateLightOn, onEnter: _ =>
            {
                Debug.Log(" [Enter LightOn]");
                StopAllCoroutines();
            }, onExit:
            state =>
            {
                lightManager.ToLight(3);
            }));
        _fsm.AddState(State.StateStayInPlaceForRandomMove, new CoState<State>(this, StateStayInPlaceForRandomMove, onEnter:_ => StopAllCoroutines()));
        _fsm.AddState(State.MoveForward, onEnter: _ =>
        {
            StartCoroutine(CoMoveForward(()=> _fsm.RequestStateChange(State.MoveBackward)));
        });
        _fsm.AddState(State.MoveBackward, onEnter: _ =>
        {
            StartCoroutine(CoMoveBackward(() => _fsm.RequestStateChange(State.StateStayInPlaceForRandomMove)));
        });
        _fsm.AddState(State.MoveToChanam, onEnter: _ => {
            MoveToTargetTr(chanamSeatTr, chanamTr, ()=> _fsm.RequestStateChange(State.StateStayInPlaceForRandomMove), useInteraction: true);
        });
        _fsm.AddState(State.MoveToJangnam, onEnter: _ => {
            MoveToTargetTr(jangnamSeatTr, jangnamTr, ()=> _fsm.RequestStateChange(State.StateStayInPlaceForRandomMove), useInteraction: true);
        });
        _fsm.AddState(State.MoveToGrandma, onEnter: _ => {
            MoveToTargetTr(grandmaSeatTr, grandmaTr, ()=> _fsm.RequestStateChange(State.StateStayInPlaceForRandomMove), useInteraction: true);
        });
        _fsm.AddState(State.MoveToChanamWife, onEnter: _ => {
            MoveToTargetTr(chanamWifeSeatTr, chanamWifeTr, ()=> _fsm.RequestStateChange(State.StateStayInPlaceForRandomMove), useInteraction: true);
        });
        
        _fsm.SetStartState(State.Wait);
        _fsm.Init();
        
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Game.WallUrgTouchDetector.HokuyoAction += OnHokuyoEvent;
    }
    
    public void TurnOffLight()
    {
        _fsm.RequestStateChange(State.Wait, true);
    }
    
    public void TurnOnLight()
    {
        Debug.Log("@@@@@@@@@@@@@@@@@ Turn On Light");
        _fsm.RequestStateChange(State.LightOn, true);
    }
    
    private void Update()
    {
        _fsm.OnLogic();
    }

    private void OnTouched()
    {
        var state = _fsm.ActiveStateName;
        if (state is State.Wait or State.LightOn or State.MoveForward)
            return;
        
        _fsm.RequestStateChange(State.MoveForward);
    }

    #region Functions
    private void MoveToTargetTr(Transform moveTarget,Transform lookTarget, Action onReached, bool useInteraction = false)
    {
        StartCoroutine(CoMoveToTarget(moveTarget, lookTarget, onReached, useInteraction));
    }
    
    IEnumerator CoMoveToTarget(Transform moveTarget, Transform lookTarget, Action onReached, bool useInteraction = false)
    {
        Debug.Log("@@@@@@@@@@@@@@@@@@@ CoMoveToTarget");
        animationMachine.ToRun();
        while (Vector3.Distance(transform.position, moveTarget.position) > moveStopDistance)
        {
            Vector3 dir = moveTarget.position - transform.position;
            dir.Normalize();
            transform.position += dir * speed * Time.deltaTime;
            animationMachine.SetLookAtTr(moveTarget);
            yield return null;
        }
       
        animationMachine.ToIdle();
        animationMachine.SetLookAtTr(lookTarget);
        if (useInteraction)
        {
            yield return animationMachine.CoInteractionOneTwo();
        }

        onReached?.Invoke();
    }

    IEnumerator CoMoveForward(Action onEnd)
    {
        moveForwardStartPos = transform.position;

        if (transform.position.x < moveForwardYTr.transform.position.x)
        {
            animationMachine.ToRun();
            Vector3 targetVector = new Vector3(transform.position.x, moveForwardYTr.transform.position.y);
            while (Vector3.Distance(transform.position, targetVector) > moveStopDistance)
            {
                Vector3 dir = targetVector - transform.position;
                dir.Normalize();
                transform.position += dir * speed * Time.deltaTime;
                yield return null;
            }
            
            animationMachine.ToIdle();
            yield return new WaitForSeconds(0.3f);
            yield return animationMachine.CoInteractionOneTwo();
        }
        onEnd?.Invoke();
    }
    
    IEnumerator CoMoveBackward(Action onEnd)
    {
        if (transform.position.x < moveForwardYTr.transform.position.x)
        {
            animationMachine.ToRun();
            while (Vector3.Distance(transform.position, moveForwardStartPos) > moveStopDistance)
            {
                Vector3 dir = moveForwardStartPos - transform.position;
                dir.Normalize();
                transform.position += dir * speed * Time.deltaTime;
                yield return null;
            }
        }
        onEnd?.Invoke();
    }
    #endregion

    //불켜기 상태
    private IEnumerator StateLightOn(CoState<State, string> arg)
    {
        yield return CoMoveToTarget(rightCandleFireTr, rightCandle.transform, null);
        Managers.Game.SendPacketLightOnRight();
        animationMachine.ToLightOn();
        yield return rightCandle.CoLightOn(1f);
        yield return CoMoveToTarget(leftCandleFireTr, leftCandle.transform, null);

        Managers.Game.SendPacketLightOnLeft();
        animationMachine.ToLightOn();
        yield return leftCandle.CoLightOn(1f);
        yield return CoMoveToTarget(waitTr, null, null);
        
        lightManager.ToLight(3);
        _fsm.StateCanExit();
        _fsm.RequestStateChange(State.StateStayInPlaceForRandomMove);
    }

    //
    private IEnumerator StateStayInPlaceForRandomMove(CoState<State, string> arg)
    {
        animationMachine.ToIdle();
        yield return new WaitForSeconds(0f);
        int random = UnityEngine.Random.Range(0, 4);
        switch (random)
        {
            case 0:
                _fsm.RequestStateChange(State.MoveToChanam);
                break;
            case 1:
                _fsm.RequestStateChange(State.MoveToJangnam);
                break;
            case 2:
                _fsm.RequestStateChange(State.MoveToGrandma);
                break;
            case 3:
                _fsm.RequestStateChange(State.MoveToChanamWife);
                break;
        }
    }
    
    private void OnMouseEvent(Define.MouseEvent obj)
    {
        if (obj != Define.MouseEvent.Click)
            return;

        //raycast가 chractercontroller 입력 받았는지 확인
        Ray ray = Managers.Game.WallCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Characters");
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.transform.gameObject == gameObject)
            {
                OnTouched();
            }
        }
    }

    
    private void OnHokuyoEvent(UrgTouchState state, Vector2 arg2)
    {
        Ray ray = Managers.Game.WallCamera.ViewportPointToRay(arg2);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Characters");
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.transform.gameObject.Equals(gameObject))
            {
                switch (state)
                {
                    case UrgTouchState.Empty:
                        break;
                    case UrgTouchState.TouchMoment:
                        break;
                    case UrgTouchState.TouchDown:
                        OnTouched();
                        break;
                    case UrgTouchState.TouchPress:
                        break;
                    case UrgTouchState.TouchPressUp:
                        break;
                    case UrgTouchState.TouchClicked:
                        break;
                }
            }
        }
    }
}
