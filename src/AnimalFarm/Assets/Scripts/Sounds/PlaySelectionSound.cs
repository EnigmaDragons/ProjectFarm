using UnityEngine;

public class PlaySelectionSound : OnMessage<PieceSelected>
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private FloatReference volume = new FloatReference(0.5f);
    
    protected override void Execute(PieceSelected msg)
    {
        if (msg.Piece.Equals(gameObject))
            source.PlayOneShot(clip, volume);
    }
}
  
