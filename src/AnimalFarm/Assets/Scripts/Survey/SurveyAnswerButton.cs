using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurveyAnswerButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;

    public void Init(string answer, Action onClick)
    {
        button.onClick.AddListener(() => onClick());
        text.text = answer;
    }
}
