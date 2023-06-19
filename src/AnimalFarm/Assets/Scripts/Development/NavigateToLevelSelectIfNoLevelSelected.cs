using UnityEngine;

public sealed class NavigateToLevelSelectIfNoLevelSelected : MonoBehaviour
{
    [SerializeField] private CurrentLevel current;
    [SerializeField] private BitVaultNavigator navigator;
    [SerializeField] private GameLevel overrideLevelStart;
    
    void Awake()
    {
        if (overrideLevelStart != null)
            current.SelectLevel(overrideLevelStart, -1, -1);
        else if (current.ActiveLevel == null)
            navigator.NavigateToLevelSelect();
    }
}
