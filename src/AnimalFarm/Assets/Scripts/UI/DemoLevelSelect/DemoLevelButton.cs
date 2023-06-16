using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoLevelButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private BoolVariable isLevelStart;
    
    public void Init(int zoneNumber, int levelNum, GameLevel level)
    {
        Init($"{levelNum + 1}", () =>
        {
            currentLevel.SelectLevel(level, zoneNumber, levelNum);
            isLevelStart.Value = true;
            Message.Publish(new StartDemoLevelRequested());
        }, level, true);
    }

    private void Init(string text, Action onClick, GameLevel level, bool available)
    {
        gameObject.SetActive(true);
        textField.text = text;
        button.onClick.AddListener(() => onClick());
        button.interactable = available;
    }
}
