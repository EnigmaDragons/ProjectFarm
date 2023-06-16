using UnityEngine;
using UnityEngine.UI;

public class UseZoneColors : GameEventTrigger
{
    [SerializeField] private CurrentZone zone;
    [SerializeField] private Image buttonSprite;

    private void Awake() => Execute();
    protected override GameEvent Trigger => zone.OnCurrentZoneChanged;
    protected override void Execute() => buttonSprite.color = zone.Zone.BackgroundColor;
}
