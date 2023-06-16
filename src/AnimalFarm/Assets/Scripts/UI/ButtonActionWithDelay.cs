using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonActionWithDelay : MonoBehaviour
{
    [SerializeField] private FloatReference delay = new FloatReference(0.6f);
    [SerializeField] private UnityEvent action;

    private void Awake() => GetComponent<Button>().onClick.AddListener(() => StartCoroutine(PerformActionWithDelay()));

    private IEnumerator PerformActionWithDelay()
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
}
  
