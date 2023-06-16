using TMPro;
using UnityEngine;

public class ZoneName : GameEventTrigger
{
    [SerializeField] private CurrentZone zone;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake() => Execute();
    protected override GameEvent Trigger => zone.OnCurrentZoneChanged;
    protected override void Execute() => text.text = zone.Zone.Name;
}
