using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : UI_Base
{
    public bool cancelOnBack;
    public PopupState state = new PopupState();
    
    protected override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
        gameObject.GetOrAddComponent<Poolable>();
        CreateRaycastBlockMask();
    }

    protected void OnResult(PopupResults result)
    {
        if (state != null)
            state.Result = result;

        Debug.Log($"{this.gameObject}:On{result.ToString()}");
        state = null;
        
        Managers.UI.ClosePopupUI(this);
        SendMessage("On" + result.ToString(), SendMessageOptions.DontRequireReceiver);
    }

    private void CreateRaycastBlockMask()
    {
        GameObject obj =  Util.FindChild(gameObject, "@RaycastBlockMask", true);
        if (obj == null)
        {
            obj = new GameObject("@RaycastBlockMask");
            obj.transform.SetParent(transform);
        }
        obj.transform.SetAsFirstSibling();
        
        RectTransform rectTransform = obj.GetOrAddComponent<RectTransform>();
        rectTransform.position = Vector3.zero;
        
        Image back = obj.GetOrAddComponent<Image>();
        Color backColor = Color.black;
        backColor.a = 0.001f;
        back.color = backColor;
        
        Util.StrechRectTransformToFullScreen(ref rectTransform);
    }
}