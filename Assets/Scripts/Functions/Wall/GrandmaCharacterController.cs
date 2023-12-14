using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using Spine.Unity.Examples;
using UniRx;
using UnityEngine;
using UnityHFSM;

public class GrandmaCharacterController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Animating,
        IsPlate,
    }
    
    [SerializeField] private string layerName;
    [SerializeField] private SkeletonAnimationHandleExample animationHandle;
    [SerializeField] private SkeletonAnimation animation;
    private StateMachine<State> _fsm;
    private Tween delay;
    
    private void Start()
    {
        _fsm = new StateMachine<State>();
        
        //idle - 기본 애니메이션 재생
        _fsm.AddState(State.Idle, onEnter:IdleOnEnter);
        
        //animating - 특정 애니메이션 재생
        _fsm.AddState(State.Animating, onEnter:AnimatingOnEnter);
        _fsm.AddState(State.IsPlate, onEnter:IsPlateOnEnter);
        _fsm.SetStartState(State.Idle);
        
        _fsm.Init();
        
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Game.WallUrgTouchDetector.HokuyoAction += OnHokuyoEvent;
        this.ObserveEveryValueChanged(_=>Managers.Game.PlateDetector.IsSomething).Subscribe(OnIsSomethingChanged);
    }

    private void Update()
    {
        _fsm.OnLogic();
    }

    private void OnIsSomethingChanged(bool b)
    {
        if (b)
        {
            _fsm.RequestStateChange(State.IsPlate);
        }
    }
    
    private void IdleOnEnter(State<State, string> obj)
    {
        animation.loop = true;
        animationHandle.PlayAnimationForState("idle", 0);
    }

    private void AnimatingOnEnter(State<State, string> obj)
    {
        animation.loop = false;
        animationHandle.PlayAnimationForState("interaction2", 0);
        var data = animationHandle.GetAnimationForState("interaction2");
        delay = DOVirtual.DelayedCall(data.Duration, () =>
        {
            _fsm.RequestStateChange(State.Idle);
        });
    }
    
    private void IsPlateOnEnter(State<State, string> obj)
    {
        animation.loop = false;
        animationHandle.PlayAnimationForState("idle2", 0);
        var data = animationHandle.GetAnimationForState("idle2");
        delay = DOVirtual.DelayedCall(data.Duration, () =>
        {
            _fsm.RequestStateChange(State.Idle);
        });
    }
    
    private void OnMouseEvent(Define.MouseEvent obj)
    {
        if (obj != Define.MouseEvent.Click)
            return;

        //raycast가 chractercontroller 입력 받았는지 확인
        Ray ray = Managers.Game.WallCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer(layerName);
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.transform.gameObject == gameObject)
            {
                if (_fsm.ActiveStateName == State.Idle)
                    _fsm.RequestStateChange(State.Animating);
            }
        }
    }

    
    private void OnHokuyoEvent(UrgTouchState state, Vector2 arg2)
    {
        Ray ray = Managers.Game.WallCamera.ViewportPointToRay(arg2);
        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer(layerName);
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
                        if (_fsm.ActiveStateName == State.Idle)
                            _fsm.RequestStateChange(State.Animating);
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
