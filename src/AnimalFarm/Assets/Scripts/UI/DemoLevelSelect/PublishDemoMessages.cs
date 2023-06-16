using UnityEngine;

public sealed class PublishDemoMessages : MonoBehaviour
{
    public void QuitLevel() => Message.Publish(new DemoQuitLevelRequested());
}
