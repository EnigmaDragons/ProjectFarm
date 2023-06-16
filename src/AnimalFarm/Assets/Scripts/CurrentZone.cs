using System;
using UnityEngine;

public class CurrentZone : ScriptableObject
{
    [SerializeField] private Campaign zones;
    [SerializeField] private GameLevels zone;
    [SerializeField] private GameEvent onCurrentZoneChanged;
    [SerializeField, ReadOnly] private int zoneIndex = 0;

    public Campaign Campaign => zones;
    public GameEvent OnCurrentZoneChanged => onCurrentZoneChanged;
    public GameLevels Zone => zone;
    public int ZoneIndex => zoneIndex;
    
    public void Init(int zoneNumber)
    {
        var clamped = Math.Max(0, Math.Min(zones.Value.Length - 1, zoneNumber));
        zoneIndex = clamped;
        zone = zones.Value[zoneIndex];
        onCurrentZoneChanged.Publish();
    }

    public void Init(Campaign campaign)
    {
        Debug.Log($"Selected {campaign.Name} Campaign");
        zones = campaign;
        Init(0);
    }
}
