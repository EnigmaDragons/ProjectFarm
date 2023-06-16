using UnityEngine;

public class PlayDestroyedSound : OnMessage<ObjectDestroyed>
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private FloatReference volume = new FloatReference(0.5f);
    
    protected override void Execute(ObjectDestroyed msg)
    {
        if (msg.Object.Equals(gameObject))
            source.PlayOneShot(clip);
    }
}
