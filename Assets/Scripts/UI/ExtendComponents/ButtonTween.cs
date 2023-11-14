using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonTweenType { Small, Long, Large }

[RequireComponent(typeof(Button))]
public class ButtonTween : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] ButtonTweenType buttonTweenType;        
    [SerializeField] Ease ease = Ease.InOutCirc;
    [SerializeField] float duration = 0.2f, elasticity = 0.5f;
    [SerializeField] int vibrato = 5;

    Vector3 smallVec = new Vector3(-0.05f, -0.05f, 0f);
    Vector3 longVec = new Vector3(-0.03f, -0.03f, 0f);
    Vector3 largeVec = new Vector3(-0.03f, -0.03f, 0f);

    private void PlayButtonTween()
    {
        Vector3 punch = Vector3.one;
        switch(buttonTweenType)
        {
            case ButtonTweenType.Small: punch = smallVec; break;
            case ButtonTweenType.Long: punch = longVec; break;
            case ButtonTweenType.Large: punch = largeVec; break;
        }

        transform.DORewind();
        transform.DOPunchScale(punch, duration, vibrato, elasticity).SetEase(ease);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayButtonTween();
    }
}