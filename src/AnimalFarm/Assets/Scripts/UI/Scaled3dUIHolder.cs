using System;
using UnityEngine;

public class Scaled3dUIHolder : MonoBehaviour
{
    [SerializeField] private FloatReference pixelsPerScale = new FloatReference(10);
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject thingToScale;

    private FloatReference _pixelsPerScale;

    private void Start() => _pixelsPerScale = pixelsPerScale;

    private void Update()
    {
        var rect = RectTransformToScreenSpace(rectTransform);
        var xScale = rect.width / _pixelsPerScale;
        var yScale = rect.height / _pixelsPerScale;
        var scale = Math.Min(xScale, yScale);
        thingToScale.transform.localScale = new Vector3(scale, scale, scale);
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        float x = transform.position.x + transform.anchoredPosition.x;
        float y = Screen.height - transform.position.y - transform.anchoredPosition.y;

        return new Rect(x, y, size.x, size.y);
    }

    public void SetPixelsPerScale(FloatReference newPixelsPerScale) => _pixelsPerScale = newPixelsPerScale;
}
