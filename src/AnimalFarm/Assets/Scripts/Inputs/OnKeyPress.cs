using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public sealed class OnKeyPress : MonoBehaviour
{
    [SerializeField] private KeyCode key;
    [SerializeField] private UnityEvent action;
    [SerializeField] private KeyCode[] alternateKeys;

    private void Update()
    {
        if (Input.anyKey)
            if (Input.GetKeyDown(key) || alternateKeys.Any(Input.GetKeyDown))
                action.Invoke();
    }
}
