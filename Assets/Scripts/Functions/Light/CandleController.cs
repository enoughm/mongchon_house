using System;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Examples;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleController : MonoBehaviour
{
    public enum CandleState
    {
        animation, // light on loop
        on, //off to on transition
        off, //off
    }

    [SerializeField] SkeletonAnimationHandleExample candleAnimationHandler;
    [SerializeField] SkeletonAnimation candleAnimation;
    [SerializeField] Light2D candleLight;
    
    private Action onLightOnComplete;
    
    public void LightOff()
    {
        StopAllCoroutines();
        var anim = candleAnimationHandler.PlayAnimationForState(CandleState.off.ToString(), 0);
        candleLight.gameObject.SetActive(false);
        candleAnimation.loop = false;
    }

    public void LightOn(out float duration, Action onComplete)
    {
        onLightOnComplete = onComplete;
        StopAllCoroutines();
        
        var anim = candleAnimationHandler.PlayAnimationForState(CandleState.on.ToString(), 0);
        candleAnimation.loop = false;
        duration = anim.Duration;
        StartCoroutine(CoLightOn(anim.Duration));
    }

    IEnumerator CoLightOn(float delay)
    {
        float time = 0;
        float ratio = 0;
        float intensityMax = 0.25f;
        float outerMax = 30;
        float innerMax = 0.7f;
        candleLight.gameObject.SetActive(true);
        while (time < delay)
        {
            time += Time.deltaTime;
            ratio = time / delay;
            candleLight.intensity = intensityMax * ratio;
            candleLight.pointLightInnerRadius = innerMax * ratio;
            candleLight.pointLightOuterRadius = outerMax * ratio;
            yield return null;
        }
        candleAnimationHandler.PlayAnimationForState(CandleState.animation.ToString(), 0);
        candleAnimation.loop = true;
        onLightOnComplete?.Invoke();
    }

}
