using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UndoButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private GameObject otherVisual;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private MoveHistory history;

    private EventSystem _eventSystem;
    
    private void OnEnable() => history.OnChanged.Subscribe(UpdateButton, this);
    private void OnDisable() => history.OnChanged.Unsubscribe(this);
    private void Awake()
    {
        _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        Hide();
    }

    private void UpdateButton()
    {
        if (history.Count < 1)
            Hide();
        else
        {
            if (text != null)
                text.text = history.Count.ToString();
            image.enabled = true;
            image.SetAllDirty();
            button.enabled = true;
            if (otherVisual != null)
                otherVisual.SetActive(true);
            _eventSystem.SetSelectedGameObject(null);
        }
    }

    private void Hide()
    {
        if (text != null)
            text.text = "";
        image.enabled = false;
        if (otherVisual != null)
            otherVisual.SetActive(false);
        button.enabled = false;
    }
}
