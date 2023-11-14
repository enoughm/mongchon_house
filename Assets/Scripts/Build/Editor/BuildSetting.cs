using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace KHJ
{
    public class BuildSetting
    {
        public static BuildSettingData _buildSettingData;

        static void SetSetting(string name)
        {
            string directory = "Resources/Settings";
            if (!Directory.Exists(Path.Combine(Application.dataPath, directory)))
                Directory.CreateDirectory(Path.Combine(Application.dataPath, directory));

            File.Copy(
                Path.Combine(Application.dataPath, string.Format("Settings/{0}.txt", name)),
                Path.Combine(Application.dataPath, directory + "/setting.txt"), true);
            AssetDatabase.Refresh();
            
            Debug.Log($"BuildSettings!! {name}");
        }
        
        
        [MenuItem("BuildSetting/WindowsBuild/DevBuild", false, 0)]
        public static void BuildDevWindows()
        {
            Builder.Build_Dev(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("BuildSetting/WindowsBuild/RealBuild", false, 0)]
        public static void BuildRealWindows()
        {
            Builder.Build_Real(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("BuildSetting/AndroidBuild/DevBuild", false, 0)]
        public static void BuildDev()
        {
            Builder.Build_Dev(BuildTarget.Android);
        }

        [MenuItem("BuildSetting/AndroidBuild/RealBuild", false, 0)]
        public static void BuildReal()
        {
            Builder.Build_Real(BuildTarget.Android);
        }

        [MenuItem("BuildSetting/BuildSetting/DevSettings", false, 1)]
        public static void SetDev()
        {
            SetSetting("DevBuildSettings");
        }

        [MenuItem("BuildSetting/BuildSetting/RealSettings", false, 1)]
        public static void SetReal()
        {
            SetSetting("RealBuildSettings");
        }

        [MenuItem("BuildSetting/SetPlayerSettings", false, 2)]
        public static void SetPlayerSettings()
        {
            UpdateSettingData();

            PlayerSettings.companyName = _buildSettingData.defaultCompany;
            PlayerSettings.productName = _buildSettingData.productName;
            PlayerSettings.applicationIdentifier = _buildSettingData.applicationIdentifier;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, _buildSettingData.applicationIdentifier);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, _buildSettingData.applicationIdentifier);
            //PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Unknown, _buildSettingData.applicationIdentifier);
            
            if (_buildSettingData.useAndroidCustomKeystore)
            {
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.keyaliasPass = _buildSettingData.keyaliasPass;
                PlayerSettings.keystorePass = _buildSettingData.keystorePass;
                PlayerSettings.Android.keyaliasPass = _buildSettingData.androidKeyaliasPass;
                PlayerSettings.Android.keystorePass = _buildSettingData.androidKeystorePass;
                PlayerSettings.Android.keystoreName = _buildSettingData.androidKeystoreName; //key store full path
                PlayerSettings.Android.keyaliasName = _buildSettingData.androidKeyaliasName;
            }
        }
        
        [MenuItem("BuildSetting/SplashSetting", false, 3)]
        public static void SetSplashImages()
        {
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.show = true;
            PlayerSettings.SplashScreen.backgroundColor = Color.black;
            PlayerSettings.SplashScreenLogo logo = new PlayerSettings.SplashScreenLogo();
            Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Settings/splashLogo.png", typeof(Texture2D));
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            logo.logo = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
            PlayerSettings.SplashScreen.logos = new[] {logo};
        }

        [MenuItem("BuildSetting/IconSetting", false, 4)]
        public static void SetAppIcons()
        {
            Texture2D appIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Settings/appIcon.png", typeof(Texture2D));
            
            BuildTargetGroup targetGroup = BuildTargetGroup.Android;
            //adaptive
            var kind = UnityEditor.Android.AndroidPlatformIconKind.Adaptive;
            var icons = PlayerSettings.GetPlatformIcons(targetGroup, kind);
            for (var i = 0; i < icons.Length; i++)
            {
                icons[i].SetTexture(appIcon);
                icons[i].SetTexture(appIcon, 1);
            }
            PlayerSettings.SetPlatformIcons(targetGroup, kind, icons);
            
            //legacy
            kind = UnityEditor.Android.AndroidPlatformIconKind.Legacy;
            icons = PlayerSettings.GetPlatformIcons(targetGroup, kind);
            for (var i = 0; i < icons.Length; i++)
                icons[i].SetTexture(appIcon);
            PlayerSettings.SetPlatformIcons(targetGroup, kind, icons);

            //rounded
            kind = UnityEditor.Android.AndroidPlatformIconKind.Round;
            icons = PlayerSettings.GetPlatformIcons(targetGroup, kind);
            for (var i = 0; i < icons.Length; i++)
                icons[i].SetTexture(appIcon);
            PlayerSettings.SetPlatformIcons(targetGroup, kind, icons);
            
            
            //Windows app Icon
            int [] sizeList = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Standalone);
            Texture2D[] iconList = new Texture2D[sizeList.Length];
            for(int i=0;i<sizeList.Length;i++)
            {
                int iconSize = sizeList[i];
                iconList[i] = appIcon;
                iconList[i].Reinitialize(iconSize,iconSize,TextureFormat.ARGB32,false);
            }
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone,iconList);
            
            //Unknown
            sizeList = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Unknown);
            iconList = new Texture2D[sizeList.Length];
            for(int i=0;i<sizeList.Length;i++)
            {
                int iconSize = sizeList[i];
                iconList[i] = appIcon;
                iconList[i].Reinitialize(iconSize,iconSize,TextureFormat.ARGB32,false);
            }
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown,iconList);
        }
        
        static void UpdateSettingData()
        {
            TextAsset asset = Resources.Load<TextAsset>("Settings/setting");
            Debug.Log($"settingJson : {asset}");
            _buildSettingData = JsonUtility.FromJson<BuildSettingData>(asset.ToString());
            if (!Enum.TryParse<Define.SettingType>(_buildSettingData.buildType, out var type))
                Debug.LogError("세팅실패!!");
        }
    }
}