using UnityEngine;

public sealed class ToggleTarget : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject[] additionalTargets;

    private bool _canToggle = true;
    
    private void OnEnable()
    {
        Message.Subscribe<LevelCompleted>(_ => _canToggle = false, this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

    public void Toggle()
    {
        if (_canToggle)
            target.Concat(additionalTargets).ForEach(t => t.SetActive(!t.activeSelf));
    }
}

