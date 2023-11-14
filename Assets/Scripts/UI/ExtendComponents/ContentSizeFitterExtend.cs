using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class ContentSizeFitterExtend : MonoBehaviour
{
    [SerializeField] RectTransform rt_Text;
    [SerializeField] ContentSizeFitter contentSize;
    [SerializeField] int minHeight = 30, maxHeight = 120;


    // Update is called once per frame
    void Update()
    {
        if (rt_Text == null || contentSize == null)
            return;

        if (rt_Text.sizeDelta.y < minHeight)
            contentSize.verticalFit = ContentSizeFitter.FitMode.MinSize;
        else if (rt_Text.sizeDelta.y >= minHeight && rt_Text.sizeDelta.y < maxHeight)
            contentSize.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        else
            contentSize.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
    }
}
