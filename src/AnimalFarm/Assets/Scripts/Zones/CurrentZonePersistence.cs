using UnityEngine;

public sealed class CurrentZonePersistence : GameEventTrigger
{
    [SerializeField] private CurrentZone zone;
    [SerializeField] private SaveStorage storage;

    private void Awake() => zone.Init(storage.GetZone());
    protected override GameEvent Trigger => zone.OnCurrentZoneChanged;
    protected override void Execute() => storage.SaveZone(zone.ZoneIndex);
}
