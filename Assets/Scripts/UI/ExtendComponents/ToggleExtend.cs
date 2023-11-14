using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleExtend : MonoBehaviour
{
    [Header(" [Toggle Event에 같이 변경될 Text Object]")]
    [SerializeField] Image iconImage;
    [SerializeField] Text childText;
    [SerializeField] TMP_Text childeText_Tmp;
    private RectTransform iconRt, textRt, textTmpRt;


    [Header(" [On/Off시 Alpha]")]
    [SerializeField] float offAlpha = 0.5f;
    [SerializeField] float onAlpha = 1f;
    
    [Header(" [On/Off시 Scale]")]
    [SerializeField] float offScale = 1f;
    [SerializeField] float onScale = 1.07f;
    
    Toggle toggle;
    [SerializeField] private float speed = 0.2f;

    private void OnValidate()
    {
        SetState_ChildObject();
    }

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { OnValueChanged(); });
        SetState_ChildObject();
    }

    void Init()
    {
        if (iconRt == null && iconImage != null)
            iconRt = iconImage.GetComponent<RectTransform>();
        if (textRt == null && childText != null)
            textRt = childText.GetComponent<RectTransform>();
        if (textTmpRt == null && childeText_Tmp != null)
            textTmpRt = childeText_Tmp.GetComponent<RectTransform>();
    }

    void SetState_ChildObject()
    {
        Init();
        if(toggle == null)
            toggle = GetComponent<Toggle>();
        
        if (childText != null)
        {
            childText.DOKill();
            textRt.DOKill();
            if (toggle.isOn)
            {
                childText.DOFade(onAlpha, speed);
                textRt.DOScale(onScale, speed);
            }
            else
            {
                childText.DOFade(offAlpha, speed);
                textRt.DOScale(offScale, speed);
            }
        }
        
        if (childeText_Tmp != null)
        {
            childeText_Tmp.DOKill();
            textTmpRt.DOKill();
            if (toggle.isOn)
            {
                childeText_Tmp.DOFade(onAlpha, speed);
                textTmpRt.DOScale(onScale, speed);
            }
            else
            {
                childeText_Tmp.DOFade(offAlpha, speed);
                textTmpRt.DOScale(offScale, speed);
            }
        }

        if (iconImage != null)
        {
            iconImage.DOKill();
            iconRt.DOKill();
            if (toggle.isOn)
            {
                iconImage.DOFade(onAlpha, speed);
                iconRt.DOScale(onScale, speed);
            }
            else
            {
                iconImage.DOFade(offAlpha, speed);
                iconRt.DOScale(offScale, speed);
            }
        }
    }


    #region Event
    void OnValueChanged()
    {
        SetState_ChildObject();
    }
    #endregion

}
