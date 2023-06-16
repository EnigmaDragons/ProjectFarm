using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneDescriptionButton : MonoBehaviour
{
    [SerializeField] private CurrentZone zone;
    [SerializeField] private Button button;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private bool _showing;
    private bool _startedShowingThisFrame;

    public void Start()
    {
        button.onClick.AddListener(Show);
    }

    public void Update()
    {
        if (_showing && Input.GetButtonUp("Fire1") && !_startedShowingThisFrame)
        {
            _showing = false;
            descriptionPanel.gameObject.SetActive(false);
        }
        else
            _startedShowingThisFrame = false;
    }

    private void Show()
    {
        if (_showing)
        {
            _showing = false;
            descriptionPanel.gameObject.SetActive(false);
        }
        else
        {
            _startedShowingThisFrame = true;
            _showing = true;
            descriptionPanel.gameObject.SetActive(true);
            descriptionText.text = Regex.Unescape(zone.Zone.Description);
        }
    }
}
