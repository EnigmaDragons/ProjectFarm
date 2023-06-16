using UnityEngine;

public class PlaySoundOnMoved : OnMessage<PieceMoved>
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private FloatReference volume = new FloatReference(0.5f);

    protected override void Execute(PieceMoved msg)
    {
        if (msg.Piece.Equals(gameObject))
            source.PlayOneShot(clip, volume);
    }
}
