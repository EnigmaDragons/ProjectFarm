using UnityEngine;

public class PlayFirstJumpedSound : OnMessage<PieceMoved, UndoPieceMoved>
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private FloatReference volume = new FloatReference(0.5f);

    private bool _isFirstJump;
    
    private void Awake() => _isFirstJump = true;

    protected override void Execute(PieceMoved msg)
    {
        if (!_isFirstJump || !msg.HasJumpedOver(gameObject)) return;
        
        source.PlayOneShot(clip, volume);
        _isFirstJump = false;
    }

    protected override void Execute(UndoPieceMoved msg)
    {
        if (!_isFirstJump && msg.HadJumpedOver(gameObject))
            _isFirstJump = true;
    }
}
