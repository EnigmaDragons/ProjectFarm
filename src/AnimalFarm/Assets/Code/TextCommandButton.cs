using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class TextCommandButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private Button _button;
    private Action _cmd = () => { };

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => _cmd());
    }

    public void Init(string commandText, Action cmd)
    {
        label.text = commandText;
        _cmd = cmd;
    }
    
    public void Select() => _button.Select();
    public void Execute() => _button.onClick.Invoke();
}
