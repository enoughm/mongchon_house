using UnityEngine;
using UnityEngine.UI;

public class LayerViewBase : ViewBase
{
    protected override void Init()
    {

    }

    public void Active(bool setActive)
    {
        this.gameObject.SetActive(setActive);
    }

    public void ActiveAsToggle()
    {
        this.gameObject.SetActive(!gameObject.activeSelf);
    }
}