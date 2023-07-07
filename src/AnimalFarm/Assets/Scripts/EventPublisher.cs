using UnityEngine;

[CreateAssetMenu]
public class EventPublisher : ScriptableObject
{
    public static void CenterOnLevel() => Message.Publish(new CenterOnLevelRequested());
    public static void ToggleLogScope(string name) => Message.Publish(new ToggleLogScope { Name = name });
}
