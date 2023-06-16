using UnityEngine;

public sealed class OnlyEnabledForMobile : MonoBehaviour
{
    [SerializeField] private GameObject target;
    
    private void Awake()
    {
        var t = target == null ? gameObject : target;
        if (!Application.isMobilePlatform)
            t.SetActive(false);
    }
}