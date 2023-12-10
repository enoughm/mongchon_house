using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] List<CandleController> candleControllers = new List<CandleController>();

    public float brightGlobalIntensity = 0.6f;
    public float darkGlobalIntensity = 0.2f;

    [Button]
    public void ToDark(float duration)
    {
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

    [Button]
    public void ToLight(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(CoToLight(duration));
    }
    
    IEnumerator CoToLight(float duration)
    {
        float time = 0;
        float targetValue = brightGlobalIntensity - darkGlobalIntensity;
        while (time < duration)
        {
            time += Time.deltaTime;
            var ratio = time / duration;
            globalLight.intensity = darkGlobalIntensity + (targetValue * ratio);
            yield return null;
        }

    }
}
