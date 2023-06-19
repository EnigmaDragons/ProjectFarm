using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public sealed class DeliveryTime2Navigator : ScriptableObject
{
    public void NavigateToGameScene() => SceneManager.LoadScene("GameScene");
    public void NavigateToMainMenu() => SceneManager.LoadScene("MainMenu");
    public void NavigateToSummaryScene() => SceneManager.LoadScene("SummaryScene");
}