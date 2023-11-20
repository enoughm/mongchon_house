using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrgTouchDetectorDebugCanvasSensingAreas : MonoBehaviour
{
    [SerializeField] private Image sensingArea;
    [SerializeField] private Image sensingRealArea;

    public float pivotOffset = -100;
    public float areaScale = 100;
    public float offsetScale = 100;
    
    public void SetData(Vector2 sensingAreaSize, Vector2 sensingRealAreaSize, Vector2 sensingAreaOffset, Vector2 sensingRealAreaOffset)
    {
        sensingAreaOffset.y *= -1;
        sensingRealAreaOffset.y *= -1;
        
        sensingAreaOffset.x *= -1;
        sensingRealAreaOffset.x *= -1;
        
        sensingArea.rectTransform.sizeDelta = sensingAreaSize * areaScale;
        sensingRealArea.rectTransform.sizeDelta = sensingRealAreaSize * areaScale;
        
        sensingArea.rectTransform.anchoredPosition = sensingAreaOffset * offsetScale;
        sensingRealArea.rectTransform.anchoredPosition = sensingRealAreaOffset * offsetScale;
        
        sensingArea.rectTransform.anchoredPosition += new Vector2(0, pivotOffset);
        sensingRealArea.rectTransform.anchoredPosition += new Vector2(0, pivotOffset);
    }
}
