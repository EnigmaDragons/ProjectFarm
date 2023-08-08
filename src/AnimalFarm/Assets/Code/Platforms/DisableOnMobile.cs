using UnityEngine;

public class DisableOnMobile : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] enableTargets;
    [SerializeField] private MonoBehaviour[] disableTargets;

    private void Awake()
    {
#if UNITY_IOS || UNITY_ANDROID
        foreach (var target in enableTargets)
            target.enabled = true;
        foreach (var target in disableTargets)
            target.enabled = false;
#elif UNITY_STANDALONE || UNITY_EDITOR
        foreach (var target in enableTargets)
            target.enabled = false;
        foreach (var target in disableTargets)
            target.enabled = true;
#endif
    }
}
