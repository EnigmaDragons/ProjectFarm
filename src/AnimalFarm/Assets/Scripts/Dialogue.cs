using UnityEngine;

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    [SerializeField] private string dialogueName;
    [SerializeField] private DialogueLine[] lines;
    [SerializeField] private Character[] startingCharacters;

    public string DialogueName => dialogueName;
    public DialogueLine[] Lines => lines;
    public Character[] StartingCharacters => startingCharacters;
}
