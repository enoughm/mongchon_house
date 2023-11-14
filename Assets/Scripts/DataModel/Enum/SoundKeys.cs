using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BgmEnum
{

}

public enum SfxEnum
{
    UI_Click,
    UI_ClickBack,
    UI_Notify,
    UI_Pop,
    UI_Switch,
    UI_Toggle
}

public enum VoiceEnum
{
    
}

public static class SoundKeysExtension
{
    public static string ToFileInfo(this SfxEnum key, out float volume)
    {
        volume = 1;
        switch (key)
        {
            case SfxEnum.UI_Click:
                return "Click";
            case SfxEnum.UI_ClickBack:
                return "ClickBack";
            case SfxEnum.UI_Notify:
                return "Notify";
            case SfxEnum.UI_Pop:
                return "Pop";
            case SfxEnum.UI_Switch:
                return "Switch";
            case SfxEnum.UI_Toggle:
                return "Toggle";
            default:
                return "";
        }
    }
    
    public static string ToFileInfo(this VoiceEnum key, out float volume)
    {
        volume = 1;
        switch (key)
        {
            default:
                return "";
        }
    }
    
    public static string ToFileInfo(this BgmEnum key, out float volume)
    {
        volume = 1;
        switch (key)
        {
            default:
                return "";
        }
    }
}