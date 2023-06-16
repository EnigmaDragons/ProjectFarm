using UnityEngine;
using UnityEngine.UI;

public class GridLayoutSwitch : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private LayoutMode layoutMode;
    [SerializeField] private Vector2 wideSize;
    [SerializeField] private Vector2 widePadding;
    [SerializeField] private int wideColumns;
    [SerializeField] private Vector2 tallSize;
    [SerializeField] private Vector2 tallPadding;
    [SerializeField] private int tallColumns;

    private void Update()
    {
        gridLayout.cellSize = layoutMode.IsTall ? tallSize : wideSize;
        gridLayout.spacing = layoutMode.IsTall ? tallPadding : widePadding;
        gridLayout.constraintCount = layoutMode.IsTall ? tallColumns : wideColumns;
        rect.sizeDelta = new Vector2(layoutMode.IsTall 
            ? tallColumns * tallSize.x + (tallColumns - 1) * tallPadding.x 
            : wideColumns * wideSize.x + (wideColumns - 1) * widePadding.x, rect.sizeDelta.y);
    }
}
