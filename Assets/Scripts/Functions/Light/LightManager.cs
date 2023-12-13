using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] List<CandleController> candleControllers = new List<CandleController>();

    public float brightGlobalIntensity = 0.6f;
    public float darkGlobalIntensity = 0.2f;
    
    public bool isEveryLightOn = false;

    public void ToDark(float duration)
    {
        isEveryLightOn = false;
        StopAllCoroutines();
        StartCoroutine(CoToDark(duration));
    }

    IEnumerator CoToDark(float duration)
    {
        candleControllers.ForEach(c => c.LightOff());

        float time = 0;
        float targetValue = brightGlobalIntensity - darkGlobalIntensity;
        while (time < duration)
        {
            time += Time.deltaTime;
            var ratio = time / duration;
            globalLight.intensity = brightGlobalIntensity - (targetValue * ratio);
            yield return null;
        }
    }

    public void ToLightLeft()
    {
        Debug.Log("TO LIGHT LEFT");
        candleControllers[0].LightOn(1f,() =>
        {
        });
    }

    public void ToLightRight()
    {
        Debug.Log("TO LIGHT RIGHT");
        candleControllers[1].LightOn(1f,() =>
        {
            ToGlobalLight(3);
        });
    }
    
    public void ToGlobalLight(float duration)
    {
        isEveryLightOn = true;
        StopAllCoroutines();
        StartCoroutine(CoToLight(duration));
    }
    
    IEnumerator CoToLight(float duration)
    {
        float time = 0;
        float targetValue = brightGlobalIntensity - darkGlobalIntensity;
        globalLight.intensity = darkGlobalIntensity;
        Debug.Log("TO Global Light");
        while (time < duration)
        {
            time += Time.deltaTime;
            var ratio = time / duration;
            globalLight.intensity = darkGlobalIntensity + (targetValue * ratio);
            yield return null;
        }
        globalLight.intensity = brightGlobalIntensity;
    }
}