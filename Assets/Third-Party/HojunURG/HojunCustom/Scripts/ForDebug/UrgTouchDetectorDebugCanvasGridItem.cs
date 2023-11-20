using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrgTouchDetectorDebugCanvasGridItem : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Text count;
    
    public void SetData(UrgGridData data)
    {
        count.text = data.sensedDataCountAverage.ToString("F1");
        Color color = Color.white;

        if (data.isHighAverageGrid)
        {
            color = Color.red;
        }
        else if(data.isCompetitionGrid)
        {
            color = Color.yellow;
        }
        else if (data.isAnyDataGrid)
        {
            color = Color.cyan;
        }
        else if (data.isIgnoredGrid)
        {
            color = Color.grey;
        }
        else
        {
            color = Color.clear;
            color.a = 0;
        }

        //     switch (data.touchState)
        // {
        //     case UrgTouchState.Empty:
        //         color = Color.clear;
        //         color.a = 0;
        //         break;
        //     case UrgTouchState.TouchDown:
        //         color = Color.gray;
        //         break;
        //     case UrgTouchState.TouchPress:
        //         color = Color.black;
        //         color.a = 0.2f;
        //         break;
        //     case UrgTouchState.TouchPressUp:
        //         color = Color.red;
        //         break;
        //     case UrgTouchState.TouchClicked:
        //         color = Color.magenta;
        //         break;
        // }
        
        bgImage.color = color;
    }
}
