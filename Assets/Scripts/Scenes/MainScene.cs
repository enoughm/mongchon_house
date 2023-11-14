using System;
using UnityEngine;

public class MainScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Main;

        Managers.UI.ShowView<MainView>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PopupState popup = Managers.UI.ShowPopupUI<CommonPopup>().Open(CommonPopup.CommonPopupType.B, "dd", "dddd", 3);
            popup.OnCancel = p =>
            {
                Debug.Log($"OnCancel");
            };
        
            popup.OnOK = p =>
            {
                Debug.Log($"OnOK");
            };
        }
    }

    public override void Clear()
    {
        Debug.Log("TestScene Clear!");
    }
}