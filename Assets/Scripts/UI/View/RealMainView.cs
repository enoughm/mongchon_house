using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RealMainView : RootViewBase
{
    private enum B { }
    private enum T { }
    private enum G { }
    private enum I { }
    private enum L { }
    
    
    private void Awake() => Init();
    protected override void Init()
    {
        base.Init();

        Bind<Button>(typeof(B));
        Bind<Text>(typeof(T));
        Bind<GameObject>(typeof(G));
        Bind<Image>(typeof(I));
        Bind<LayerViewBase>(typeof(L));
    }

    private void OnEnable()
    {
        
    }
}