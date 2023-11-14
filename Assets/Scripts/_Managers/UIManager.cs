using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    int _popupOrder = 10;

    Stack<PopupBase> _popupStack = new Stack<PopupBase>();
    Stack<string> _viewStack = new Stack<string>();
    RootViewBase _rootView = null;
    
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void Init()
    {
        Managers.Input.KeyboardAction -= OnEscapeKeyToBackViewUI;
        Managers.Input.KeyboardAction += OnEscapeKeyToBackViewUI;
        
        Managers.Input.KeyboardAction -= OnEscapeKeyToClosePopupUI;
        Managers.Input.KeyboardAction += OnEscapeKeyToClosePopupUI;
    }



    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        
        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(Managers.Setting.BuildSettingData.resolutionWidth, Managers.Setting.BuildSettingData.resolutionHeight);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

        if (sort)
        {
            canvas.sortingOrder = _popupOrder;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T ShowView<T>(string name = null) where T : RootViewBase
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        if (_rootView != null)
        {
            if (_rootView.name == name)
                return null;

            _viewStack.Push(_rootView.name);
            CloseViewUI();
        }

        GameObject go = Managers.Resource.Instantiate($"UI/View/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _rootView = sceneUI;

        if (_rootView.clearViewStack)
        {
            _viewStack.Clear();
        }

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public void BackView()
    {
        if (_viewStack.Count == 0)
            return;
        
        Debug.Log("BackViewUI()");
        CloseViewUI();
        
        string viewName = _viewStack.Pop();
        ShowView<RootViewBase>(viewName);
    }

    public void CloseViewUI()
    {
        if (_rootView == null)
            return;
        
        Managers.Resource.Destroy(_rootView.gameObject);
        _rootView = null;
    }
    
    
    private void OnEscapeKeyToBackViewUI(Define.KeyEvent keyEvent)
    {
        if (keyEvent != Define.KeyEvent.Escape)
            return;

        if (_popupStack.Count > 0)
            return;

        if (_rootView == null)
            return;

        if(_rootView.cancelOnBack)
            BackView();
    }
    
    public T ShowPopupUI<T>(string name = null) where T : PopupBase
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup); 
        
        go.transform.SetParent(Root.transform);

        popup.state = new PopupState();;
        _popupOrder++;

        return popup;
    }
    
    public void ClosePopupUI(PopupBase target = null)
    {
        if (_popupStack.Count == 0)
            return;

        if (target != null && _popupStack.Peek().name != target.name)
        {
            Debug.Log($"Close Popup Failed");
            return;
        }

        PopupBase popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _popupOrder--;
    }

    private void OnEscapeKeyToClosePopupUI(Define.KeyEvent keyEvent)
    {
        if (keyEvent != Define.KeyEvent.Escape)
            return;
        
        if (_popupStack.Count == 0)
            return;
        
        PopupBase popup = _popupStack.Peek();
        if(popup.cancelOnBack)
            ClosePopupUI();
    }
    
    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _rootView = null;
    }
}
