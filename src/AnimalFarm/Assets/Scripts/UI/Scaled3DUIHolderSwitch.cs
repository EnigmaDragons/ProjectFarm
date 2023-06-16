using UnityEngine;

public class Scaled3DUIHolderSwitch : MonoBehaviour
{
    [SerializeField] private Scaled3dUIHolder holder;
    [SerializeField] private LayoutMode layoutMode;
    [SerializeField] private FloatReference tallPixelsPerScale;
    [SerializeField] private FloatReference widePixelsPerScale;

    public void Update() => holder.SetPixelsPerScale(layoutMode.IsTall ? tallPixelsPerScale : widePixelsPerScale);
}
