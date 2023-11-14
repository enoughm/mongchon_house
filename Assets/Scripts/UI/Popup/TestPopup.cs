using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestPopup : PopupBase
{
    enum Buttons
    {
        CancelButton,
        OkButton,
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

        GetB((int)Buttons.CancelButton).gameObject.BindEvent(_ =>
        {
            OnResult(PopupResults.Cancel);
        });

        GetB((int)Buttons.OkButton).gameObject.BindEvent(_ =>
        {
            OnResult(PopupResults.OK);
        });
        
        //GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        //BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);
    }

}