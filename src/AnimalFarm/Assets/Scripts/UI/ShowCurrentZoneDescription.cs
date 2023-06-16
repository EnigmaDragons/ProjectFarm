using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public sealed class ShowCurrentZoneDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private CurrentZone zone;

    private void OnEnable()
    {
        UpdateText();
        zone.OnCurrentZoneChanged.Subscribe(UpdateText, this);
    }

    private void OnDisable()
    {
        zone.OnCurrentZoneChanged.Unsubscribe(this);
    }

    private void UpdateText()
    {
        display.text = Regex.Unescape(zone.Zone?.Description ?? "");
    }
}
