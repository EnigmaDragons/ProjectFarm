using UnityEngine;
using UnityEngine.UI;

public sealed class UseThemeImageTint : MonoBehaviour
{
    [SerializeField] private CurrentTheme theme;
    [SerializeField] private Image image;
    [SerializeField] private ThemeElement element;
    [SerializeField] private bool fullyOpaque;

    private void Awake()
    {
        var baseColor = theme.ColorFor(element);
        image.color = new Color(baseColor.r, baseColor.g, baseColor.b, image.color.a);
        if (fullyOpaque)
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    } 
}
