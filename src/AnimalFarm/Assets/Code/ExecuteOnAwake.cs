using UnityEngine;
using UnityEngine.Events;

public sealed class ExecuteOnAwake : MonoBehaviour
{
    [SerializeField] private UnityEvent action;

    private void Awake() => action.Invoke();
}
