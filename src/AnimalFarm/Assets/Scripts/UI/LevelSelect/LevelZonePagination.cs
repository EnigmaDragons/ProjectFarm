using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelZonePagination : MonoBehaviour
{
    [SerializeField] private LevelZoneButtons buttons;
    [SerializeField] private GameObject controls;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private TextMeshProUGUI pageNumText;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Image nextPageButtonImage;
    [SerializeField] private SaveStorage storage;
    [SerializeField] private CurrentZone zone;
    [SerializeField] private TutorialButton tutorialButton;
    [SerializeField] private GoToStoryButton storyButton;
    [SerializeField] private UnlockNewZoneRequirements locks;

    private Campaign Campaign => zone.Campaign;
    private int ZoneCount => Campaign.Value.Length;
    private int _zoneIndex;

    public void Awake()
    {
        Init(storage.GetZone());
        nextPageButton.GetComponent<Button>().onClick.AddListener(NextPage);
        previousPageButton.GetComponent<Button>().onClick.AddListener(PreviousPage);
    }

    private void Init(int page) => Change(page);
    public void PreviousPage() => Change(_zoneIndex - 1);
    
    public void NextPage()
    {
        if (!locks.IsLocked)
            Change(_zoneIndex + 1);
    }
    
    private void Change(int newIndex)
    {
        _zoneIndex = Math.Min(Math.Max(newIndex, 0), Campaign.Value.Length - 1);
        zone.Init(_zoneIndex);
        Render();
    }

    private void Render()
    {
        buttons.Init(_zoneIndex, Campaign.Value[_zoneIndex]);
        tutorialButton.Init(_zoneIndex, Campaign);
        storyButton.Init(Campaign.Value[_zoneIndex]);
        controls.SetActive(ZoneCount > 1);
        previousPageButton.interactable = _zoneIndex != 0;
        var hasAnotherZone =  _zoneIndex < ZoneCount - 1;
        nextPageButtonImage.enabled = hasAnotherZone;
        nextPageButton.interactable = hasAnotherZone;
        if (pageNumText != null)
            pageNumText.text = (_zoneIndex + 1).ToString();
    }
}
