using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class FlashButtonChildOnClick : MonoBehaviour
{
    [SerializeField] private GameObject inner;
    [SerializeField] private int numFlashes = 2;
    [SerializeField] private float singleFlashDuration = 0.10f;

    private bool _isFlashing;

    private void Awake() => GetComponent<Button>().onClick.AddListener(() => StartCoroutine(BeginFlashing()));
    
    private IEnumerator BeginFlashing()
    {
        if (_isFlashing) yield break;
        
        _isFlashing = true;
        var segmentDuration = singleFlashDuration / 2;
        for (var i = 0; i < numFlashes; i++)
        {
            inner.SetActive(false);
            yield return new WaitForSeconds(segmentDuration);
            inner.SetActive(true);
            yield return new WaitForSeconds(segmentDuration);
        }

        _isFlashing = false;
    }
}
