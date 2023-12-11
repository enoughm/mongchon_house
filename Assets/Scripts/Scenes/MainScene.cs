using System;
using UniRx;
using UnityEngine;

public class MainScene : BaseScene
{

    [SerializeField] private GameObject strechToFillUI;
    
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            strechToFillUI.SetActive(!strechToFillUI.activeSelf);
        }
    }

    public override void Clear()
    {
        Debug.Log("TestScene Clear!");
    }
}