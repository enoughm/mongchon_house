using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace KHJ
{
    public class Builder : MonoBehaviour
    {
        static string _buildDate = "", _outputPath = "_OUTPUT";
        static BuildTarget _buildTarget;

        public static void Build_Dev(BuildTarget buildTarget)
        {
            _buildTarget = buildTarget;
            BuildSetting.SetDev();
            if (_buildTarget != EditorUserBuildSettings.activeBuildTarget)
            {
                if (EditorUtility.DisplayDialog("[경고]", "Editor BuildTarget과 현재 설정된 BuildTarget과 다릅니다. 그래도 진행 하겠습니까?", "진행", "취소"))
                {
                    Build();
                }
            }
            else
                Build();
        }
        
        public static void Build_Real(BuildTarget buildTarget)
        {
            _buildTarget = buildTarget;
            BuildSetting.SetReal();
            if (_buildTarget != EditorUserBuildSettings.activeBuildTarget)
            {
                if (EditorUtility.DisplayDialog("[경고]", "Editor BuildTarget과 현재 설정된 BuildTarget과 다릅니다. 그래도 진행 하겠습니까?", "진행", "취소"))
                {
                    Build();
                }
            }
            else
                Build();
        }


        static void Build_Jenkins()
        {
            _buildDate = CommandLineReader.GetCustomArgument("buildDate");
            //isTestBuild = bool.Parse(CommandLineReader.GetCustomArgument("isTestBuild"));
            _buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), CommandLineReader.GetCustomArgument("buildTarget"));

            Build();
        }
        
        static void Build()
        {
            try
            {
                BuildSetting.SetAppIcons();
                BuildSetting.SetSplashImages();
                BuildSetting.SetPlayerSettings();
                
                string outputFile = "", fileExtension = "";
                if (string.IsNullOrEmpty(_buildDate))
                    _buildDate = DateTime.Now.ToString("yyyyMMdd");

                if (_buildTarget == BuildTarget.StandaloneWindows64)
                {
                    _outputPath = Path.Combine(_outputPath, $"{Application.productName}_{_buildDate}");
                    fileExtension = "exe";
                }
                else
                {
                    fileExtension = "apk";
                }

                if (!Directory.Exists(_outputPath))
                    Directory.CreateDirectory(_outputPath);

                var fileName = $"{Application.productName}_{_buildDate}.{fileExtension}";
                outputFile = string.Format("{0}/{1}", _outputPath, fileName);
                Debug.LogFormat("**** outputFile : {0}", outputFile);

                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                buildPlayerOptions.scenes = BuildSupport.FindEnableScenes();
                buildPlayerOptions.locationPathName = outputFile;
                buildPlayerOptions.target = _buildTarget;
                buildPlayerOptions.options = BuildOptions.None;

                var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                var summary = report.summary;
                Debug.LogFormat("**** summary.result : {0}", summary.result);

                if (summary.result == BuildResult.Succeeded)
                {
                    Debug.Log("====> Build Succeeded!");
                }
                else if (summary.result == BuildResult.Failed)
                {
                    Debug.LogError("====> Build Failed!");
                    foreach (var step in report.steps)
                    {
                        foreach (var message in step.messages)
                        {
                            Debug.LogError("****" + message);
                        }
                    }
                }
                else if (summary.result == BuildResult.Cancelled)
                {
                    Debug.LogError("====> Build Cancelled!");
                }
                else
                {   // Unknown
                    Debug.LogError("====> Build Unknown Error! ");
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogError("====> [Scene File FoundException] : " + ex.Message);
                Debug.LogError(ex.StackTrace.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogError("====> Build failed. [Exception] : " + ex.Message);
                Debug.LogError(ex.StackTrace.ToString());
            }
            finally
            {
                Debug.Log("====> git reset execute!!!");
            }
        }
    }

}