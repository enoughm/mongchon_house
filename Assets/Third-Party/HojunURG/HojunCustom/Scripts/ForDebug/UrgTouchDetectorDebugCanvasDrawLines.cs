using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UrgTouchDetectorDebugCanvasDrawLines : MonoBehaviour
{
    public Transform origin;
    public Image linePrefab;
    public List<Image> lines = new List<Image>();

    public float scale = 2;
    public float offsetY = -100;


    public void SetData(List<long> distances, float angleDelta, float angleOffset)
    {
        if (distances.Count == 0)
            return;
        
        if(lines.Count != distances.Count)
            CreateLines(distances.Count);
        try
        {
            if (distances.Count > 0)
            {
            
                var right = origin.right;
                var forward = origin.forward;


                for (int i = 0; i < distances.Count; i++)
                {
                    var angle = i * angleDelta - angleOffset;
                    var x = Mathf.Cos(angle * Mathf.Deg2Rad);
                    var y = Mathf.Sin(angle * Mathf.Deg2Rad);
                    var d = distances[i] * scale;

                    var pos = (x * right + y * forward) * d;
                
                    lines[i].gameObject.SetActive(true);
                    lines[i].rectTransform.sizeDelta = new Vector2(1, d);
                    lines[i].transform.eulerAngles = new Vector3(0, 0, angle);
                    
                }
            }
            else
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    lines[i].gameObject.SetActive(false);
                }
            }
        }
        catch (Exception e)
        {
            
        }
    }

    private void CreateLines(int count)
    {
        linePrefab.gameObject.SetActive(true);
        while (lines.Count < count)
        {
            var line = Instantiate(linePrefab, origin);
            lines.Add(line);
        }
        linePrefab.gameObject.SetActive(false);
    }
}
