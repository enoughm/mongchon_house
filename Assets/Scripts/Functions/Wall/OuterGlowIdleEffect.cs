using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OuterGlowIdleEffect : MonoBehaviour
{
    [Header("IDLE")]
    public float idleDuration = 1f;
    public float alphaMin = 0.5f;
    public float alphaMax = 0.55f;
    public Ease alphaEase = Ease.Linear;

    private CanvasGroup _canvasGroup;
    private Image _trueShadow;

    private void Awake()
    {
        _trueShadow = GetComponentInChildren<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }
    
    public void BeginIdleEffect()
    {
        _trueShadow.gameObject.SetActive(true);
        _canvasGroup.DOFade(alphaMax, idleDuration).From(alphaMin).SetLoops(-1, LoopType.Yoyo).SetEase(alphaEase).OnStepComplete(
            () =>
            {
            });
    }
    
    public void StopIdleEffect()
    {
        _canvasGroup.DOKill();
        _trueShadow.gameObject.SetActive(false);
    }
}
