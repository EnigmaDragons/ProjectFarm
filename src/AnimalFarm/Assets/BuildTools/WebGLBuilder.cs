#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.Windows;

public static class WebGLBuilder {
    
    public static void Build() {

        string[] scenes = {
            "Assets/Scenes/SimpleLogoScene.unity", 
            "Assets/Scenes/GameScene.unity",
            "Assets/Scenes/LevelSelectScene.unity",
            "Assets/Scenes/CreditsScene.unity",
            "Assets/Scenes/RewardScene.unity",
            "Assets/Scenes/MainMenu.unity",
        };

        var buildPath = "C:/Temp/Builds/BitVault-WebGL/";

        Directory.Delete(buildPath);
        Directory.CreateDirectory(buildPath);
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.WebGL, BuildOptions.None);      
    }
}
#endif
