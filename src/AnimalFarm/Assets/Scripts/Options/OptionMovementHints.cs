using UnityEngine;
using UnityEngine.UI;

public class OptionMovementHints : MonoBehaviour
{
    [SerializeField] private SaveStorage saveStorage;
    [SerializeField] private Toggle toggle;
    [SerializeField] private BoolVariable option;

    private void OnEnable()
    {
        var initialState = saveStorage.GetShowMovementHints();
        toggle.isOn = initialState;
        option.Value = initialState;
        toggle.onValueChanged.AddListener(SetValue);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(SetValue);
    }

    private void SetValue(bool isActive)
    {
        option.Value = isActive;
        saveStorage.SetShowMovementHints(isActive);
    }
}
