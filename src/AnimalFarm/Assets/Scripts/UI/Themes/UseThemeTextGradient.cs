using TMPro;
using UnityEngine;

public sealed class UseThemeTextGradient : MonoBehaviour
{
    [SerializeField] private CurrentTheme theme;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private ThemeElement tintElement;
    [SerializeField] private ThemeElement gradientElement;

    private void Awake()
    {
        text.color = theme.ColorFor(tintElement);
        var gradientColors = theme.Current.GradientFor(gradientElement);
        text.colorGradientPreset = gradientColors;
    }
}
