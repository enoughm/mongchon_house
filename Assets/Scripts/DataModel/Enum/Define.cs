using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    public enum Scene
    {
        Unknown,
        Main,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Down,
        Press,
        Click,
    }
    
    public enum KeyEvent
    {
        Any,
        Escape,
    }

    public enum CameraMode
    {
        QuarterView,
    }

    public enum Language
    {
        Unknown,
        English,
        Korean,
    }

    public enum SettingType
    {
        Dev,
        Real
    }
}
