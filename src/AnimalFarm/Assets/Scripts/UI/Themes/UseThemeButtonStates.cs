using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class UseThemeButtonStates : MonoBehaviour
{
    [SerializeField] private CurrentTheme theme;
    [SerializeField] private ThemeElement normalTint;
    [SerializeField] private ThemeElement hoverTint;
    [SerializeField] private ThemeElement pressedTint;

    void Awake()
    {
        var button = GetComponent<Button>();
        var sourceColors = button.colors;
        var colorsBlock = new ColorBlock
        {
            colorMultiplier = sourceColors.colorMultiplier,
            disabledColor = sourceColors.disabledColor,
            fadeDuration = sourceColors.fadeDuration,
            normalColor = theme.ColorFor(normalTint),
            highlightedColor = theme.ColorFor(hoverTint),
            pressedColor = theme.ColorFor(pressedTint),
            selectedColor = sourceColors.selectedColor
        };
        button.colors = colorsBlock;
    }
}
