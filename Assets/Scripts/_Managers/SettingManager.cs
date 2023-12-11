using System;
using System.IO;
using UnityEngine;

public class SettingManager
{
    public Define.SettingType buildType;
    public BuildSettingData BuildSettingData;
    

    public void Init()
    {
        TextAsset asset = Resources.Load<TextAsset>("Settings/setting");
        Debug.Log($"settingJson : {asset}");
        BuildSettingData = JsonUtility.FromJson<BuildSettingData>(asset.ToString());
        if (!Enum.TryParse(BuildSettingData.buildType, out buildType))
            Debug.LogError("세팅실패!!");

        Setting(buildType);
    
        Debug.Log($"세팅완료!! : {BuildSettingData.buildType} => {buildType.ToString()}");
    }

    void Setting(Define.SettingType type)
    {
        DebugSetting(type);
        Application.runInBackground = true;
        if(!Application.isEditor)
            WindowHelper.MakeTopMost();
    }

    void DebugSetting(Define.SettingType type)
    {
        switch (type)
        {
            case Define.SettingType.Dev:
                Application.logMessageReceived -= LogHandler;
                Application.logMessageReceived += LogHandler;
                break;
            case Define.SettingType.Real:
                Application.logMessageReceived -= LogHandler;
                break;
        }
    }
    
    void LogHandler(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
            Debug.LogError(condition + "\n\n" + stackTrace);
    }
}
