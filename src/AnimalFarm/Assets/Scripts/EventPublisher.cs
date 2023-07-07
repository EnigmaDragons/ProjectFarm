using UnityEngine;

[CreateAssetMenu]
public class EventPublisher : ScriptableObject
{
    public static void CenterOnLevel() => Message.Publish(new CenterOnLevelRequested());
}
