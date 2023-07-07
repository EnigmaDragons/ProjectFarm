using UnityEngine;

public sealed class DestroyIfEaten : OnMessage<PieceMoved>
{
    [SerializeField] private UiSfxPlayer sfx;
    [SerializeField] private AudioClipWithVolume sound;
    
    protected override void Execute(PieceMoved msg)
    {
        var g = gameObject;
        if (!msg.HasEaten(gameObject)) return;
        
        Log.SInfo("Gameplay", $"Destroying {g.name} because it was eaten by {msg.Piece.name}", g);
        sfx.Play(sound.clip, sound.volume);
        Message.Publish(new ObjectDestroyed(g, msg.MoveNumber, false));
    }

    public void Revert()
    {
        gameObject.SetActive(true);
    }
}
