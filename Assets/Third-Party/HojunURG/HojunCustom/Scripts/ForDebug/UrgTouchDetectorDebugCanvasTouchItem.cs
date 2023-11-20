using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UrgTouchDetectorDebugCanvasTouchItem : MonoBehaviour
{
    [SerializeField] private Image cursorImage;
    [SerializeField] private Image clickImagePrefab;
    //private List<Image> clickImageList = new List<Image>();

    private void Awake()
    {
        clickImagePrefab.gameObject.SetActive(false);
    }

    public void SetData(RealTouchData realTouchData, Vector2 screenSize)
    {
        var x = realTouchData.viewPortPos.x * screenSize.x;
        var y = realTouchData.viewPortPos.y * screenSize.y;

        var sizeWidth = cursorImage.rectTransform.sizeDelta.x * 0.5f;
        cursorImage.rectTransform.anchoredPosition = new Vector2(x - sizeWidth, y - sizeWidth);
        
        switch (realTouchData.touchState)
        {
            case UrgTouchState.Empty:
                break;
            case UrgTouchState.TouchDown:
                ObjectOnOff(Color.black, x, y);
                break;
            case UrgTouchState.TouchPress:
                break;
            case UrgTouchState.TouchPressUp:
                ObjectOnOff(Color.red, x, y);
                break;
            case UrgTouchState.TouchClicked:
                ObjectOnOff(Color.cyan, x, y);
                break;
        }
        
        
    }

    public void ActiveCursor(bool useCursor)
    {
        cursorImage.gameObject.SetActive(useCursor);
    }
    
    private void ObjectOnOff(Color targetColor, float x, float y)
    {
        StartCoroutine(CoObjectOnOff(targetColor, x, y));
    }

    IEnumerator CoObjectOnOff(Color targetColor, float x, float y)
    {
        clickImagePrefab.gameObject.SetActive(true);
        var instance = Instantiate(clickImagePrefab, transform);
        clickImagePrefab.gameObject.SetActive(false);
        cursorImage.transform.SetAsLastSibling();
        instance.color = targetColor;
        
        var width = instance.rectTransform.sizeDelta.x* 0.5f;
        instance.rectTransform.anchoredPosition = new Vector2(x - width, y - width);
        //clickImageList.Add(instance);
        float time = 0;
        float duration = 1f;
        while (time < duration)
        {
            time += Time.deltaTime;
            var alpha = 1f - time / duration;
            instance.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
            yield return null;
        }
        //clickImageList.Remove(instance);
        DestroyImmediate(instance.gameObject);
    }
}
