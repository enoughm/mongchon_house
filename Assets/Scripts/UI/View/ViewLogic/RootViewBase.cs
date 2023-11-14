using System;
using UnityEngine;
using UnityEngine.UI;

public class RootViewBase : ViewBase
{
    public bool cancelOnBack = true;
    public bool clearViewStack = false;
    
    protected override void Init()
    {
        Managers.UI.SetCanvas(gameObject, false);
    }
}