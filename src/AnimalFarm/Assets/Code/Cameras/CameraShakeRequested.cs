
public class CameraShakeRequested
{
    public float Duration { get; }
    public float Amount { get; }
    
    public CameraShakeRequested(float duration, float amount)
    {
        Duration = duration;
        Amount = amount;
    }
}
