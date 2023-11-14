using System;
using UnityEngine;
using UnityEngine.UI;

public class MainView : RootViewBase
{
    enum Buttons
    {
        GoTestViewButton,
        GoMainViewButton,
        GoLoginViewButton,
    }

    enum Texts
    {
        PointText,
        ScoreText,
    }

    enum GameObjects
    {
        TestObject,
    }

    enum Images
    {
        ItemIcon,
    }

    enum View
    {
        UnderView,
    }

    private void Awake() => Init();

    protected override void Init()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<LayerViewBase>(typeof(View));
        
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
            GetL((int)View.UnderView).ActiveAsToggle();
        }, Define.UIEvent.Click);
        
        GetL((int)View.UnderView).Active(false);
    }
}