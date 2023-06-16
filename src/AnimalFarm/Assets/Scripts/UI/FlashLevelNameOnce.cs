using System.Collections;
using TMPro;
using UnityEngine;

public class FlashLevelNameOnce : MonoBehaviour
{
    [SerializeField] private float durationBeforeHide = 4f;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CurrentLevel level;

    void Awake()
    {
        text.text = level.ActiveLevel.Name;
        if (level.ActiveLevel.IsTutorial)
            gameObject.SetActive(false);
    }

    void Start() => StartCoroutine(HideAfterDelay());

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(durationBeforeHide);
        gameObject.SetActive(false);
    }
}
