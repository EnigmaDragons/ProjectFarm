using UnityEngine;
using UnityEngine.UI;

public class ToggleGameObject : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Button enableButton;
    [SerializeField] private Button disableButton;

    void OnEnable()
    {
        if (enableButton != null)
            enableButton.onClick.AddListener(EnableTarget);
        if (disableButton != null)
            disableButton.onClick.AddListener(DisableTarget);
    }

    private void OnDisable()
    {
        if (enableButton != null)
            enableButton.onClick.RemoveListener(EnableTarget);
        if (disableButton != null)
            disableButton.onClick.RemoveListener(DisableTarget);
    }

    public void DisableTarget() => target.SetActive(false);
    public void EnableTarget()
    {
        target.SetActive(true);
    }
}
