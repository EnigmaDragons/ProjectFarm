using UnityEngine;
using System;

public sealed class DemoLevelSelectionUI : OnMessage<StartDemoLevelRequested, DemoQuitLevelRequested>
{
    [SerializeField] private DemoLevelButtons buttons;
    [SerializeField] private CurrentZone zone;
    [SerializeField] private DemoTutorialButton tutorialButton;
    [SerializeField] private GameObject[] children;

    private Campaign Campaign => zone.Campaign;
    private int _zoneIndex;

    public void Start()
    {
        _zoneIndex = Math.Min(Math.Max(0, 0), Campaign.Value.Length - 1);
        zone.Init(_zoneIndex);
        Render();
    }

    private void Render()
    {
        buttons.Init(_zoneIndex, Campaign.Value[_zoneIndex]);
        tutorialButton.Init(_zoneIndex, Campaign);
    }

    protected override void Execute(StartDemoLevelRequested msg)
    {
        children.ForEach(c => c.SetActive(false));
    }

    protected override void Execute(DemoQuitLevelRequested msg) => Enable();

    private void Enable()
    {
        children.ForEach(c => c.SetActive(true));
    }
}
