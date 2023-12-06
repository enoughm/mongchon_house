using System;
using UniRx;
using UnityEngine;

public class MainScene : BaseScene
{
    
    [SerializeField] private LightManager _lightManager;
    [SerializeField] private BugFactory _bugFactory;
    
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Main;

        Managers.UI.ShowView<MainView>();
        
        _lightManager.ToDark(1);
    }

    private void Start()
    {
        _lightManager.ToDark(1);
        this.ObserveEveryValueChanged(_ => Managers.Game.IsSomeone).Subscribe(_=>OnChangeIsSomeone(_));
    }

    private void OnChangeIsSomeone(bool isSomeone)
    {
        if (isSomeone)
        {
            _lightManager.ToLight(3);
        }
        else
        {
            _lightManager.ToDark(3);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _lightManager.ToLight(1);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            _lightManager.ToDark(1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            _bugFactory.MadeBug();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            _bugFactory.ClearBugs();
        }
    }

    public override void Clear()
    {
        Debug.Log("TestScene Clear!");
    }
}