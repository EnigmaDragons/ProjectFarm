using UnityEngine;
using UnityEngine.UI;

public class ZoneLogo : GameEventTrigger
{
    [SerializeField] private CurrentZone currentZone;
    [SerializeField] private Image logo;

    private void Awake() => Execute();
    protected override GameEvent Trigger => currentZone.OnCurrentZoneChanged;
    
    protected override void Execute()
    {
        var zone = currentZone.Zone;
        logo.sprite = zone.Logo;
        logo.color = new Color(zone.LogoColor.r, zone.LogoColor.g, zone.LogoColor.b, logo.color.a);
    }
}
