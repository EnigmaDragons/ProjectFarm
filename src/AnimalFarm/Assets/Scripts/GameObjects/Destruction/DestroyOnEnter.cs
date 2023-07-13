using UnityEngine;

public class DestroyOnEnter : OnMessage<PieceMovementFinished>
{
    [SerializeField] private UiSfxPlayer sfx;
    [SerializeField] private AudioClipWithVolume sound;
    
    protected override void Execute(PieceMovementFinished msg)
    {        
        if (msg.MovementType != MovementType.Enter || msg.Object != gameObject) return;
        
        if (sfx != null && sound != null)
            sfx.Play(sound.clip, sound.volume);
        Message.Publish(new ObjectDestroyed(gameObject, msg.MoveNumber, false));
    }

    public void Revert()
    {
        gameObject.SetActive(true);
    }
}
