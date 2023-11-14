using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildSupport
{
    public static string[] FindEnableScenes()
    {
        List<string> enableScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                enableScenes.Add(scene.path);
                Debug.Log($"{scene.path}");
            }
        }

        return enableScenes.ToArray();
    }

    public static void ExecuteGitReset(string workingDirectory)
    {
        ExecuteCommand("git reset --hard", workingDirectory);
    }

    static void ExecuteCommand(string command, string workingDirectory)
    {
        System.Diagnostics.ProcessStartInfo processInfo;
        System.Diagnostics.Process process;

        processInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c " + command);
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        // *** Redirect the output ***
        processInfo.RedirectStandardError = true;
        processInfo.RedirectStandardOutput = true;
        processInfo.WorkingDirectory = workingDirectory;

        process = System.Diagnostics.Process.Start(processInfo);
        process.WaitForExit();
        process.Close();
    }
}
