using UnityEngine;

[CreateAssetMenu]
public class ConjoinedDialogues : ScriptableObject
{
    [SerializeField] private Dialogue intro;
    [SerializeField] private Dialogue outro;

    public Dialogue Intro => intro;
    public Dialogue Outro => outro;
}
