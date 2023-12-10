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
    [SerializeField] Light2D candleHighlightLight;

    public float intensityMax = 0.25f;
    public float innerMax = 0.7f;
    public float outerMax = 20;
    
    public float subIntensityMax = 0.4f;
    
    private Action onLightOnComplete;
    
    public void LightOff()
    {
        StopAllCoroutines();
        var anim = candleAnimationHandler.PlayAnimationForState(CandleState.off.ToString(), 0);
        candleLight.gameObject.SetActive(false);
        candleHighlightLight.gameObject.SetActive(false);
        candleAnimation.loop = false;
    }

    public void LightOn(float delay, Action onComplete)
    {
        onLightOnComplete = onComplete;
        StopAllCoroutines();
        StartCoroutine(CoLightOn(delay));
    }

    IEnumerator CoLightOn(float delay)
    {
        yield return new WaitForSeconds(delay);
        var anim = candleAnimationHandler.PlayAnimationForState(CandleState.on.ToString(), 0);
        candleAnimation.loop = false;
        
        float time = 0;
        candleLight.gameObject.SetActive(true);
        candleHighlightLight.gameObject.SetActive(true);
        while (time < anim.Duration + 1f)
        {
            time += Time.deltaTime;
            var ratio = time / anim.Duration;
            candleLight.intensity = intensityMax * ratio;
            candleLight.pointLightInnerRadius = innerMax * ratio;
            candleLight.pointLightOuterRadius = outerMax * ratio;
            
            
            candleHighlightLight.intensity = subIntensityMax * ratio;
            yield return null;
        }
        candleAnimationHandler.PlayAnimationForState(CandleState.animation.ToString(), 0);
        candleAnimation.loop = true;
        onLightOnComplete?.Invoke();
    }

}
