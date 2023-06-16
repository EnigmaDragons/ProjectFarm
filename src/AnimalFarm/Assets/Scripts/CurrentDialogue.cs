using UnityEngine;

[CreateAssetMenu]
public class CurrentDialogue : ScriptableObject
{
    [SerializeField] private Maybe<ConjoinedDialogues> dialogue;

    public Maybe<ConjoinedDialogues> Dialogue => dialogue;

    public void Set(Maybe<ConjoinedDialogues> story) => dialogue = story;
}
