using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

public class MultiPlatformBuild
{
    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        string[] scenes = { "Assets/Yoyo/Project/3.GasSys/1.Scenes/GasSysScene.unity" };
        CreateDirectory("Builds/Windows");
        BuildPlayer(scenes, "Builds/Windows/가스계소화설비.exe", BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Build MacOS")]
    public static void BuildMacOS()
    {
        string[] scenes = { "Assets/Scenes/MainScene.unity" };
        CreateDirectory("Builds/MacOS");
        BuildPlayer(scenes, "Builds/MacOS/MyGame.app", BuildTarget.StandaloneOSX);
    }

    [MenuItem("Build/Build Linux")]
    public static void BuildLinux()
    {
        string[] scenes = { "Assets/Scenes/MainScene.unity" };
        CreateDirectory("Builds/Linux");
        BuildPlayer(scenes, "Builds/Linux/MyGame.x86_64", BuildTarget.StandaloneLinux64);
    }

    [MenuItem("Build/Build Android")]
    public static void BuildAndroid()
    {
        string[] scenes = { "Assets/Yoyo/Project/3.GasSys/1.Scenes/GasSysScene.unity" };
        CreateDirectory("Builds/Android");
        BuildPlayer(scenes, "Builds/Android/MyGame.apk", BuildTarget.Android);
    }

    [MenuItem("Build/Build iOS")]
    public static void BuildiOS()
    {
        string[] scenes = { "Assets/Scenes/MainScene.unity" };
        CreateDirectory("Builds/iOS");
        BuildPlayer(scenes, "Builds/iOS", BuildTarget.iOS);
    }

    private static void BuildPlayer(string[] scenes, string path, BuildTarget buildTarget)
    {
        BuildReport report = BuildPipeline.BuildPlayer(scenes, path, buildTarget, BuildOptions.None);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log($"Build succeeded: {summary.totalSize} bytes");
        }
        else if (summary.result == BuildResult.Failed)
        {
            UnityEngine.Debug.LogError("Build failed");
        }
    }

    private static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}
