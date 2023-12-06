using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] CandleController middleCandleController;

    
    public void ToDark(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(CoToDark(duration));
    }

    IEnumerator CoToDark(float duration)
    {
        middleCandleController.LightOff();
        float time = 0;
        float darkIntensity = 0.2f;
        float fromIntensity = 1f;
        float ratio = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            ratio = time / duration;
            globalLight.intensity = fromIntensity - (fromIntensity - darkIntensity) * ratio;
            yield return null;
        }
    }
    
    public void ToLight(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(CoToLight(duration));
    }
    
    IEnumerator CoToLight(float duration)
    {
        middleCandleController.LightOn(out var delay, null);
        yield return new WaitForSeconds(delay * 0.66f);
        float time = 0;
        float darkIntensity = 0.5f;
        float lightIntensity = 1f;
        float ratio = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            ratio = time / duration;
            globalLight.intensity = darkIntensity + (lightIntensity - darkIntensity) * ratio;
            yield return null;
        }

    }
}
