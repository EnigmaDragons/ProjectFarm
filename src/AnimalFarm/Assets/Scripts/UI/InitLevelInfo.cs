using TMPro;
using UnityEngine;

public class InitLevelInfo : MonoBehaviour
{
    [SerializeField] private CurrentDialogue dialogue;
    [SerializeField] private BoolReference isLevelStart;
    [SerializeField] private TextMeshProUGUI label;

    private void Awake()
    {
        var name = isLevelStart.Value
            ? dialogue.Dialogue.Value.Intro.DialogueName
            : dialogue.Dialogue.Value.Outro.DialogueName;
        label.text = $"{name}";
    }
}
