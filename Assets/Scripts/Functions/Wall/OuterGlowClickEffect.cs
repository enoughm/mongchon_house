using DG.Tweening;
using LeTai.TrueShadow;
using Sirenix.OdinInspector;
using UnityEngine;

public class OuterGlowClickEffect : MonoBehaviour
{
    private TrueShadow _trueShadow;
    private CanvasGroup _canvasGroup;

    public Ease ease = Ease.InBack;
    public Ease alphaEase = Ease.InBack;
    public float duration = 1f;
    public float startSize = 40f;
    public float endSize = 240f;
    
    public float spread = 0.85f;

    private Tween _tween;
    
    private void Awake()
    {
        _trueShadow = GetComponentInChildren<TrueShadow>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _trueShadow.Size = startSize;
        _trueShadow.Spread = spread;
        _canvasGroup.alpha = 0;
    }


    [Button]
    public void TouchEffect()
    {
        _tween?.Kill();
        _canvasGroup?.DOKill();
        
        float hitValue = startSize;
        _trueShadow.Spread = spread;
        _tween = DOTween.To(() => hitValue, x => hitValue = x, endSize, duration).SetEase(ease).OnUpdate(() =>
        {
            _trueShadow.Size = hitValue;
        });
        _canvasGroup.DOFade(0, duration).From(1).SetEase(alphaEase);
    }
    
}
