using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoryArchiveButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;

    public void Init(string buttonText, UnityAction onClick)
    {
        text.text = buttonText;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);
        button.gameObject.SetActive(!string.IsNullOrWhiteSpace(buttonText));
    }
}
