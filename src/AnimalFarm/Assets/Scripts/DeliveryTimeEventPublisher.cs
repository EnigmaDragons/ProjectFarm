using UnityEngine;

[CreateAssetMenu]
public sealed class DeliveryTimeEventPublisher : ScriptableObject
{
    public void ShowLevelSelect() => Message.Publish(new ShowLevelSelect());
    public void GoToNextLevel() => Message.Publish(new GoToNextLevel());
    public void RetryLevel() => Message.Publish(new RetryLevel());
}

