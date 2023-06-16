using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockNewZoneRequirements : MonoBehaviour
{
    [SerializeField] private SaveStorage storage;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CurrentZone currentZone;
    [SerializeField] private GameObject locked;
    [SerializeField] private Button nextZoneButton;
    [SerializeField] private BoolVariable developmentToolsEnabled;

    private Campaign campaign => currentZone.Campaign;
    private int _zone = 0;

    public bool IsLocked => locked.activeSelf;
    
    private void OnEnable()
    {
        UpdateRequirements();
    }

    private void Update()
    {
        if (_zone != storage.GetZone())
            UpdateRequirements();
    }

    private void UpdateRequirements()
    {
        _zone = storage.GetZone();

        if (campaign.Value.Length == _zone + 1 || developmentToolsEnabled.Value)
        {
            Unlock();
        }
        else if (storage.GetLevelsCompletedInZone(campaign.Value[_zone]) < campaign.Value[_zone].Value.Length)
        {
            if (text != null)
                text.text = $"{campaign.Value[_zone].Value.Length - storage.GetLevelsCompletedInZone(campaign.Value[_zone])} Levels";
            Lock();
        }
        else if (storage.GetTotalStars() < campaign.Value[_zone + 1].StarsRequired)
        {
            if (text != null)
                text.text = $"{campaign.Value[_zone + 1].StarsRequired} Data Cubes";
            Lock();
        }
        else if (DateTimeOffset.Now.CompareTo(campaign.Value[_zone + 1].MinDateRequired) < 0)
        {
            if (text != null)
                text.text = $"Unlocks on {campaign.Value[_zone + 1].MinDateRequired.ToLocalTime():g}";
            Lock();
        }
        else
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        if (text != null)
            text.text = "";
        locked.SetActive(false);
        nextZoneButton.interactable = true;
    }

    private void Lock()
    {
        locked.SetActive(true);
        nextZoneButton.interactable = false;
    }
}
