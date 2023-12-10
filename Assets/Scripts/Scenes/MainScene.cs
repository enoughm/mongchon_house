using System;
using UniRx;
using UnityEngine;

public class MainScene : BaseScene
{

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Main;
        Managers.UI.ShowView<MainView>();
    }

    private void Start()
    {
    }

    private void Update()
    {

    }

    public override void Clear()
    {
        Debug.Log("TestScene Clear!");
    }
}