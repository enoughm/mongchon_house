using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnderView : LayerViewBase
{
    enum Buttons
    {
        GoTestViewButton,
        GoMainViewButton,
        GoLoginViewButton,
    }


    enum Texts
    {

    }

    enum GameObjects
    {
        
    }

    enum Images
    {
        
    }

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
        
        GetB((int)Buttons.GoTestViewButton).gameObject.BindEvent(_ =>
        {
            Debug.Log("GoTestViewButton");
            Managers.UI.ShowView<TestView>();
        });

        GetB((int)Buttons.GoMainViewButton).gameObject.BindEvent(_ =>
        {
            Debug.Log("GoTestViewButton");
            Managers.UI.ShowView<MainView>();
        });
        
        GetB((int)Buttons.GoLoginViewButton).gameObject.BindEvent(_ =>
        {
            Debug.Log("GoTestViewButton");
            Managers.UI.ShowView<LoginView>();
        });
    }

    private void OnEnable()
    {
        
    }
}