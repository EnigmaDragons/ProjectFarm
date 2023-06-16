using UnityEngine;
using UnityEngine.UI;

public class ZoneBorderColor : GameEventTrigger
{
    [SerializeField] private CurrentZone zone;
    [SerializeField] private Image image;

    protected override GameEvent Trigger => zone.OnCurrentZoneChanged;
    protected override void Execute() => image.color = zone.Zone.BackgroundColor;
}
