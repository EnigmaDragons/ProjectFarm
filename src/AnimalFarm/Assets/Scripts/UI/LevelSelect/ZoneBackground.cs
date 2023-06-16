using UnityEngine;
using UnityEngine.UI;

public class ZoneBackground : GameEventTrigger
{
    [SerializeField] private CurrentZone zone;
    [SerializeField] private Image background;

    private void Awake() => Execute();
    protected override GameEvent Trigger => zone.OnCurrentZoneChanged;
    protected override void Execute()
    {
        var color = zone.Zone.BackgroundColor;
        background.color = new Color(color.r, color.g, color.b, background.color.a);
    }
}
