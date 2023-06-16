using UnityEngine;

public class PlaneDistanceSwitch : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private LayoutMode layout;
    [SerializeField] private float tallPlaneDistance;
    [SerializeField] private float widePlaneDistance;

    private void Update() => canvas.planeDistance = layout.IsTall ? tallPlaneDistance : widePlaneDistance;
}
