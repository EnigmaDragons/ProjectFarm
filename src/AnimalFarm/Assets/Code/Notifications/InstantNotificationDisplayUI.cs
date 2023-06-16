using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class InstantNotificationDisplayUI : OnMessage<ShowNotification, DismissNotification>
{
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private Image panel;

    private void Awake()
    {
        display.text = "";
        Hide();
    }

    protected override void Execute(ShowNotification msg)
    {
        if (string.IsNullOrWhiteSpace(msg.Text))
            Hide();
        else
        {
            display.text = msg.Text;
            display.DOKill();
            display.color = display.color.WithAlpha(0);
            display.DOFade(1f, 1.5f);
            panel.DOKill();
            panel.DOFade(1f, 1.5f);
        }
    }

    protected override void Execute(DismissNotification msg) => Hide();

    private void Hide()
    {
        panel.DOKill();
        panel.DOFade(0, 1.5f);
        display.DOKill();
        display.DOFade(0, 1.5f);
    }
}
