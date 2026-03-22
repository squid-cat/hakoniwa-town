using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.Linq;

public class BuildScript
{
    [MenuItem("Build/Configure Windows Settings")]
    public static void ConfigureWindowsSettings()
    {
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.resizableWindow = true;
    }

    private static readonly string[] Scenes = GetEnabledScenes();

    private static string[] GetEnabledScenes()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    [MenuItem("Build/Build WebGL")]
    public static void BuildWebGL()
    {
        string buildPath = GetArgValue("-buildPath", "../frontend/public");
        Build(BuildTarget.WebGL, buildPath);
    }

    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        // Configure windowed mode
        ConfigureWindowsSettings();
        
        string buildPath = GetArgValue("-buildPath", "Builds/Windows/HakoniwaTown.exe");
        Build(BuildTarget.StandaloneWindows64, buildPath);
    }

    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        BuildWebGL();
        BuildWindows();
    }

    private static void Build(BuildTarget target, string path)
    {
        Debug.Log($"Building {target} to {path}...");

        var options = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = path,
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {summary.totalSize / 1024 / 1024} MB");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed!");
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }

    private static string GetArgValue(string argName, string defaultValue)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == argName && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }
        return defaultValue;
    }
}
