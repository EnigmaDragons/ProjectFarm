using UnityEngine;
using UnityEngine.UI;

public class SetToggleToCurrentDeveloperMode : MonoBehaviour
{
    [SerializeField] private BoolVariable developerModeEnabled;
    [SerializeField] private Toggle toggle;
    
    private void OnEnable() => toggle.SetIsOnWithoutNotify(developerModeEnabled.Value);
}
