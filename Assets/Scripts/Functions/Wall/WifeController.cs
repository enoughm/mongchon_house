using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using Spine;
using Spine.Unity;
using Spine.Unity.Examples;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityHFSM;

public class WifeController : MonoBehaviour
{
    public enum State
    {
        Idle,
        Animating,
    }
    
    [SerializeField] private string layerName;
    [SerializeField] private GameObject backAnimationObject;
    
    [SerializeField] private SkeletonAnimationHandleExample frontAnimationHandle;
    [SerializeField] private SkeletonAnimation frontAnimation;

    [SerializeField] private OuterGlowIdleEffect outerGlowIdleEffect;
    [SerializeField] private OuterGlowClickEffect outerGlowClickEffect;
    
    private StateMachine<State> _fsm;
    private Tween delay;
    
    private void Start()
    {
        _fsm = new StateMachine<State>();
        
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

    private void Update()
    {
        _fsm.OnLogic();
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
    
    private void IdleOnEnter(State<State, string> obj)
    {
        delay?.Kill();
        frontAnimationHandle.gameObject.SetActive(false);
        backAnimationObject.gameObject.SetActive(true);
        if(Managers.Game.lightState)
            outerGlowIdleEffect.BeginIdleEffect();
    }

    private void AnimatingOnEnter(State<State, string> obj)
    {
        delay?.Kill();
        outerGlowIdleEffect.StopIdleEffect();
        backAnimationObject.gameObject.SetActive(false);
        frontAnimationHandle.gameObject.SetActive(true);
        frontAnimation.loop = false;
        frontAnimation.state.TimeScale = 1f;
        frontAnimationHandle.PlayAnimationForState("1", 0);
        var data = frontAnimationHandle.GetAnimationForState("1");
        delay = DOVirtual.DelayedCall(data.Duration - 0.2f, () =>
        {
            RewindSpineAnimation();
            DOVirtual.DelayedCall(data.Duration- 0.05f, () =>
            {
                _fsm.RequestStateChange(State.Idle);
            });

        });
    }
    
    void RewindSpineAnimation()
    {
        frontAnimation.state.ClearTracks();
        TrackEntry entry = frontAnimation.state.AddAnimation(1, "1", false, 0);
        entry.Reverse = true;
    }
    
    int StringToHash (string s) {
        return Animator.StringToHash(s);
    }
    
    private void OnMouseEvent(Define.MouseEvent obj)
    {
        if (!Managers.Game.lightState)
            return;
        
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
                Managers.Sound.PlaySfx(SFX.TouchSound);
                outerGlowClickEffect?.TouchEffect();
                        
                if(_fsm.ActiveStateName == State.Idle)
                    _fsm.RequestStateChange(State.Animating, true);
            }
        }
    }

    
    private void OnHokuyoEvent(UrgTouchState state, Vector2 arg2)
    {
        if (!Managers.Game.lightState)
            return;

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
                        Managers.Sound.PlaySfx(SFX.TouchSound);
                        outerGlowClickEffect?.TouchEffect();
                        
                        if(_fsm.ActiveStateName == State.Idle)
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