using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CommonPopup : PopupBase
{
    public enum CommonPopupType
    {
        A,  //auto close type(no button)
        B,  //ok, cancel type
        C   //close type
    }
    
    enum Buttons
    {
        OkButton,
        CancelButton,
        YesButton,
        NoButton,
        CloseButton
    }

    enum Texts
    {
        MessageText,
        TitleText,
    }

    enum GameObjects
    {
        Buttons
    }

    enum Images
    {
    }
    
    float autoCloseTime = 2f;

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        GetB((int)Buttons.CancelButton).gameObject.BindEvent(_ =>OnResult(PopupResults.Cancel));
        GetB((int)Buttons.OkButton).gameObject.BindEvent(_ => OnResult(PopupResults.OK));
        GetB((int)Buttons.YesButton).gameObject.BindEvent(_ => OnResult(PopupResults.Yes));
        GetB((int)Buttons.NoButton).gameObject.BindEvent(_ => OnResult(PopupResults.No));
        GetB((int)Buttons.CloseButton).gameObject.BindEvent(_ => OnResult(PopupResults.Close));

        GetT((int)Texts.TitleText);
    }

    private void OnEnable()
    {
        //layout refresh
        StartCoroutine(LayoutRefresh());
    }

    public PopupState Open(CommonPopupType popupType, string message, string title = "알림", float autoCloseTime = 2f)
    {
        PopupResults results = PopupResults.None;
        switch (popupType)
        {
            case CommonPopupType.B:
                results = PopupResults.OK | PopupResults.Cancel;
                break;
            case CommonPopupType.C:
                results = PopupResults.Close;
                break;
        }
        this.autoCloseTime = autoCloseTime;
        GetT((int)Texts.TitleText).text = title; 
        GetT((int)Texts.MessageText).text = message;
        SetActiveObjs(popupType, results);
        return state;
    }

    private void SetActiveObjs(CommonPopupType popupType, PopupResults results)
    {
        switch (popupType)
        {
            case CommonPopupType.A:
                StartCoroutine(AutoClose());
                break;
            case CommonPopupType.B:
            case CommonPopupType.C:
                GetB((int)Buttons.OkButton).gameObject.SetActive((results & PopupResults.OK) == PopupResults.OK);
                GetB((int)Buttons.CancelButton).gameObject.SetActive((results & PopupResults.Cancel) == PopupResults.Cancel);
                GetB((int)Buttons.YesButton).gameObject.SetActive((results & PopupResults.Yes) == PopupResults.Yes);
                GetB((int)Buttons.NoButton).gameObject.SetActive((results & PopupResults.No) == PopupResults.No);
                GetB((int)Buttons.CloseButton).gameObject.SetActive((results & PopupResults.Close) == PopupResults.Close);
                break;
        }
        GetG((int)GameObjects.Buttons).SetActive(popupType != CommonPopupType.A);
    }
    
    IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(autoCloseTime);
        OnResult(PopupResults.Close);
    }
    
    IEnumerator LayoutRefresh() {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.GetChild(0).transform);
    }
}