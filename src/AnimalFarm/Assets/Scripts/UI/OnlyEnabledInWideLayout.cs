using System.Collections;
using UnityEngine;

public sealed class OnlyEnabledInWideLayout : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private LayoutMode layout;
    [SerializeField] private bool checkAfterAwake = false;

    private void OnEnable()
    {
        UpdateState();
        if (isActiveAndEnabled && checkAfterAwake)
            StartCoroutine(UpdateEverySecond(1));
    }

    private IEnumerator UpdateEverySecond(float intervalSeconds)
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(intervalSeconds);
            UpdateState();
        }
    }
    
    private void UpdateState()
    {
        target.SetActive(layout.IsWide);
    }
}
