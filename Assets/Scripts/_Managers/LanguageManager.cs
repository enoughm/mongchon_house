using System;
using I2.Loc;
using UnityEngine;

public class LanguageManager
{
    private Define.Language defaultLanguage = Define.Language.English;
    private const string saveKey = "LanguageSaveKey";
    
    public void Init()
    {
        LoadCurrentLanguage();
        //string text1 = ScriptLocalization.Test;  
    }
    
    public Define.Language GetCurrentLanguage()
    {
        string[] langNames = Enum.GetNames(typeof(Define.Language));

        for (int i = 0; i < langNames.Length; i++)
        {
            if (langNames[i].Equals(LocalizationManager.CurrentLanguage))
            {
                return (Define.Language)Enum.Parse(typeof(Define.Language), langNames[i]);
            }
        }
        
        return Define.Language.Unknown;
    }
    
    public bool TryChangeLanguage(Define.Language toLanguage)
    {
        if (LocalizationManager.HasLanguage(toLanguage.ToString()))
        {
            LocalizationManager.CurrentLanguage = toLanguage.ToString();
            SaveCurrentLanguage();
            return true;
        }

        return false; 
    }

    private void LoadCurrentLanguage()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            TryChangeLanguage((Define.Language) PlayerPrefs.GetInt(saveKey));
        }
        else
        {
            TryChangeLanguage(defaultLanguage);
        }
    }
    
    private void SaveCurrentLanguage()
    {
        PlayerPrefs.SetInt(saveKey, (int)GetCurrentLanguage());
    }
}
