using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using Sirenix.OdinInspector;
using Spine.Unity;
using Spine.Unity.Examples;
using UnityEngine;
using UnityHFSM;

public class NormalAnimationCharacterStateMachine : MonoBehaviour
{
    public enum State
    {
        Idle,
        Animating,
    }
    
    [SerializeField] private string layerName;
    [SerializeField] private SkeletonAnimationHandleExample animationHandle;
    [SerializeField] private SkeletonAnimation animation;

    [SerializeField] private OuterGlowIdleEffect outerGlowIdleEffect;
    [SerializeField] private OuterGlowClickEffect outerGlowClickEffect;
    
    private StateMachine<State> _fsm;
    private Vector3 initPos;
    private Tween delayTween;
    
    
    private void Start()
    {
        _fsm = new StateMachine<State>();
        
        initPos = animation.transform.position;
        
        //idle - 기본 애니메이션 재생
        _fsm.AddState(State.Idle, onEnter:IdleOnEnter);
        
        //animating - 특정 애니메이션 재생
        _fsm.AddState(State.Animating, onEnter:AnimatingOnEnter);
        
        _fsm.SetStartState(State.Idle);
        
        _fsm.Init();
        
        
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.Game.WallUrgTouchDetector.HokuyoAction += OnHokuyoEvent;
        Managers.Game.onLightStateChanged += OnLightStateChanged;
        outerGlowIdleEffect.StopIdleEffect();
    }

    private void OnLightStateChanged(bool obj)
    {
        if (obj)
        {
            outerGlowIdleEffect.BeginIdleEffect();
        }
        else
        {
            outerGlowIdleEffect.StopIdleEffect();
        }
    }

    private void Update()
    {
        _fsm.OnLogic();
        
    }

    private void IdleOnEnter(State<State, string> obj)
    {
        delayTween?.Kill();
        animation.loop = true;
        animationHandle.PlayAnimationForState("1", 0);
    }

    private void AnimatingOnEnter(State<State, string> obj)
    {
        delayTween?.Kill();
        animation.loop = false;
        animationHandle.PlayAnimationForState("2", 0);
        Managers.Sound.PlaySfx(SFX.TouchSound);
        var data = animationHandle.GetAnimationForState("2");
        delayTween = DOVirtual.DelayedCall(data.Duration, () =>
        {
            _fsm.RequestStateChange(State.Idle);
        });
        
        outerGlowClickEffect?.TouchEffect();
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
                _fsm.RequestStateChange(State.Animating, true);
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
                        _fsm.RequestStateChange(State.Animating, true);
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
