using UnityEngine;
using UnityEngine.UI;

public class TestView : RootViewBase
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
            Managers.UI.ShowView<TestView>();
        });

        GetB((int)Buttons.GoMainViewButton).gameObject.BindEvent(_ =>
        {
            Managers.UI.ShowView<MainView>();
        });
        
        GetB((int)Buttons.GoLoginViewButton).gameObject.BindEvent(_ =>
        {
            Managers.UI.ShowView<LoginView>();
        });
    }
}