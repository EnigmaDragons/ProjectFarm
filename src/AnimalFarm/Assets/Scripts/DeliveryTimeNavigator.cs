using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public sealed class DeliveryTimeNavigator : ScriptableObject
{
    public void NavigateToGameScene() => SceneManager.LoadScene("GameScene");
    public void NavigateToMainMenu() => SceneManager.LoadScene("MainMenu");
    public void NavigateToSummaryScene() => SceneManager.LoadScene("SummaryScene");
}
