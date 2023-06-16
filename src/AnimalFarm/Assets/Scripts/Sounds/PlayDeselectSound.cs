using UnityEngine;

public class PlayDeselectSound : OnMessage<PieceDeselected>
{
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private AudioClip clip;
    
    protected override void Execute(PieceDeselected msg) => player.Play(clip);
}