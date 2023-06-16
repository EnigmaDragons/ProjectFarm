using System.Collections;
using TMPro;
using UnityEngine;

public class LoopingScrollingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float scrollSpeed = 10.0f;
    [SerializeField] private float yOffset;

    private TextMeshProUGUI cloneText;
    private RectTransform textRectTransform;

    private void Awake()
    {
        textRectTransform = text.GetComponent<RectTransform>();
        cloneText = Instantiate(text) as TextMeshProUGUI;
        RectTransform cloneRectTransform = cloneText.GetComponent<RectTransform>();
        cloneRectTransform.SetParent(textRectTransform);
        cloneRectTransform.anchorMin = new Vector2(0.5f, 1);
        cloneRectTransform.localPosition = new Vector3(0, text.preferredHeight, cloneRectTransform.position.z);
        cloneRectTransform.localScale = new Vector3(1, 1, 1);
        cloneText.text = text.text;
    }

    private IEnumerator Start()
    {
        float height = text.preferredHeight;
        Vector3 startPosition = textRectTransform.localPosition;
        float scrollPosition = 0;
        while (true)
        {
            textRectTransform.localPosition = new Vector3(startPosition.x, (scrollPosition % height) + yOffset, startPosition.z);
            scrollPosition += scrollSpeed * Time.deltaTime;
            yield return null;
        }
    }
}