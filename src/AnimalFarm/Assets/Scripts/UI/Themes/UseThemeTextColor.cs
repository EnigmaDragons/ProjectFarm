using TMPro;
using UnityEngine;

public sealed class UseThemeTextColor : MonoBehaviour
{
    [SerializeField] private CurrentTheme theme;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private ThemeElement element;

    private void Awake() => text.color = theme.ColorFor(element);
}
