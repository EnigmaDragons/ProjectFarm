using UnityEngine;
using UnityEngine.UI;

public class ZoneTiledLogo : GameEventTrigger
{
    [SerializeField] private CurrentZone zone;
    [SerializeField] private Image logo;

    private void Awake() => Execute();
    protected override GameEvent Trigger => zone.OnCurrentZoneChanged;
    protected override void Execute()
    {
        logo.sprite = zone.Zone.LogoTiled;
        logo.color = new Color(logo.color.r, logo.color.g, logo.color.b, logo.color.a);
    }
}
