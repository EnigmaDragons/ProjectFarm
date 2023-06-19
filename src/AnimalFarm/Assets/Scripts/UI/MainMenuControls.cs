using UnityEngine;
using UnityEngine.UI;

public class MainMenuControls : MonoBehaviour
{
    [SerializeField] private BitVaultNavigator navigator;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private SaveStorage saveStorage;

    private void Awake()
    {
        saveStorage.Init();
        continueButton.interactable = saveStorage.HasStartedGame();
        continueButton.onClick.AddListener(() => navigator.NavigateToLevelSelect());
    }
}
