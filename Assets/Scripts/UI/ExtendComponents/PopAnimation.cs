using System;
using DG.Tweening;
using UnityEngine;

namespace M2S
{
    public class PopAnimation : MonoBehaviour
    {
        public enum PopupAnimationType
        {
            None,
            PopCommon,   
        }
        
        [SerializeField] Vector3 initScale;
        [SerializeField] private PopupAnimationType _animationType = PopupAnimationType.PopCommon;

        private RectTransform rt;
        
        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            rt.localScale = initScale;
        }

        private void OnEnable()
        {
            switch (_animationType)
            {
                case PopupAnimationType.None:
                    break;
                case PopupAnimationType.PopCommon:
                    AnimType_PopCommon();
                    break;
                default:
                    break;
            }
        }

        private void AnimType_PopCommon()
        {
            rt.localScale = initScale;
            rt.DOKill();
            rt.DOScale(1, 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                rt.DOScale(new Vector3(1, 0.9f, 0), 0.3f).SetEase(Ease.OutCirc).OnComplete(() =>
                {
                    rt.DOScale(1, 0.2f).SetEase(Ease.InOutQuint);
                });
            });
        }
    }
}