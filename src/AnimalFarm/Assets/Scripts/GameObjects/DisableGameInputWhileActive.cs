using UnityEngine;

public class DisableGameInputWhileActive : MonoBehaviour
{
    [SerializeField] private LockBoolVariable gameInputActive;

    private void OnEnable() => gameInputActive.Lock(gameObject);
    private void OnDisable() => gameInputActive.Unlock(gameObject);
}
