using UnityEngine;

[CreateAssetMenu]
public class EventPublisher : ScriptableObject
{
    public static void CenterOnLevel() => Message.Publish(new CenterOnLevelRequested());
    public static void ToggleLogScope(string name) => Message.Publish(new ToggleLogScope { Name = name });
    public static void GoToNextLevel() => Message.Publish(new GoToNextLevel());
    public static void RetryLevel() => Message.Publish(new LevelResetRequested());
}
