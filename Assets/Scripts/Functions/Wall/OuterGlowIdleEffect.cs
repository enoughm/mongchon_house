using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LeTai.TrueShadow;
using UnityEngine;

public class OuterGlowIdleEffect : MonoBehaviour
{
    [Header("IDLE")]
    public float idleDuration = 1f;
    public float alphaMin = 0.5f;
    public float alphaMax = 0.55f;
    public float spread = 0.85f;
    public Ease alphaEase = Ease.Linear;

    private CanvasGroup _canvasGroup;
    private TrueShadow _trueShadow;

    private void Awake()
    {
        _trueShadow = GetComponentInChildren<TrueShadow>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _trueShadow.Spread = spread;
    }
    
    public void BeginIdleEffect()
    {
        _trueShadow.gameObject.SetActive(true);
        _canvasGroup.DOFade(alphaMax, idleDuration).From(alphaMin).SetLoops(-1, LoopType.Yoyo).SetEase(alphaEase).OnStepComplete(
            () =>
            {
                _trueShadow.Spread = spread;
            });
    }
    
    public void StopIdleEffect()
    {
        _canvasGroup.DOKill();
        _trueShadow.gameObject.SetActive(false);
    }
}
