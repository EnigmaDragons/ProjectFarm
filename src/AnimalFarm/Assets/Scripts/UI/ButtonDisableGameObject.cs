using UnityEngine;
using UnityEngine.UI;

public sealed class ButtonDisableGameObject : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Button button;

    private void OnEnable() => button.onClick.AddListener(Execute);
    private void OnDisable() => button.onClick.RemoveListener(Execute);
    
    private void Execute() => target.SetActive(false);
}
