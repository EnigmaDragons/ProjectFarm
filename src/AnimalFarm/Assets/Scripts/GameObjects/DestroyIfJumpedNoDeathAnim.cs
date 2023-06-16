using UnityEngine;

public sealed class DestroyIfJumpedNoDeathAnim : OnMessage<PieceMoved>
{
    [SerializeField] private UiSfxPlayer sfx;
    [SerializeField] private AudioClipWithVolume sound;
    
    protected override void Execute(PieceMoved msg)
    {        
        if (!msg.HasJumpedOver(gameObject)) return;
        
        sfx.Play(sound.clip, sound.volume);
        Message.Publish(new ObjectDestroyed(gameObject, msg.MoveNumber, false));
    }

    public void Revert()
    {
        gameObject.SetActive(true);
    }
}
