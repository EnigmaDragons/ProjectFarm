using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class Navigator : ScriptableObject
{
    private static bool loadSynchronously;

    public static void NavigateToMainMenu() => NavigateTo("MainMenu");
    public static void NavigateToGameScene() => NavigateTo("GameScene");
    public static void ReloadGameSceneSync() => SceneManager.LoadScene("GameScene");

    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private static void NavigateTo(string name)
    {
        PlayerPrefs.Save();
        
        if (!loadSynchronously)
        {
            var loading = SceneManager.LoadSceneAsync(name);
            if (LoadingScreen.Instance != null)
                LoadingScreen.Instance.Init(loading);
        }
        else
        {
            SceneManager.LoadScene(name);
        }
    }
}
