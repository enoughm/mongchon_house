using UnityEngine;

public class MainScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Main;

        Managers.UI.ShowView<MainView>();
    }

    public override void Clear()
    {
        Debug.Log("TestScene Clear!");
    }
}