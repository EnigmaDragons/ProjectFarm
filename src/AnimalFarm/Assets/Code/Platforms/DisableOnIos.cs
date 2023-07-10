using UnityEngine;

public class DisableOnIos : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] targets;
    
    #if UNITY_IOS
    private void Awake()
    {
        foreach (var target in targets)
            target.enabled = false;
    }
    #endif
}
