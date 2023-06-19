using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [SerializeField] private DialogueLineType type;
    [SerializeField, TextArea] private string text;
    [SerializeField] private Character character;

    public DialogueLineType Type => type;
    public string Text => text;
    public Character Character => character;
}
