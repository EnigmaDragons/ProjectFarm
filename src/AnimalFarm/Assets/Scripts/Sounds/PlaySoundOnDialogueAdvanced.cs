using UnityEngine;

public sealed class PlaySoundOnDialogueAdvanced: OnMessage<DialogueAdvanced>
{
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private AudioClip clip;

    protected override void Execute(DialogueAdvanced msg) => player.Play(clip);
}
