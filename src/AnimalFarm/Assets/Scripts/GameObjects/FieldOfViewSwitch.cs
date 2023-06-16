using UnityEngine;

public class FieldOfViewSwitch : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private LayoutMode layout;
    [SerializeField] private float tallFieldOfView;
    [SerializeField] private float widFieldOfView;
    [SerializeField] private float tallTabletFieldOfView;
    [SerializeField] private float wideTabletFieldOfView;
    [SerializeField] private float tall39By18FieldOfView;
    [SerializeField] private float wide39By18FieldOfView;

    private bool isTall;
    private ResolutionAspectRatio aspectRatio;

    private void Start() => UpdateFieldOfView();

    private void Update()
    {
        if (isTall != layout.IsTall || aspectRatio != layout.AspectRatio)
            UpdateFieldOfView();
    }

    private void UpdateFieldOfView()
    {
        isTall = layout.IsTall;
        aspectRatio = layout.AspectRatio;
        if (isTall)
        {
            if (aspectRatio == ResolutionAspectRatio.Default)
                camera.fieldOfView = tallFieldOfView;
            else if (aspectRatio == ResolutionAspectRatio.FourByThree)
                camera.fieldOfView = tallTabletFieldOfView;
            else if (aspectRatio == ResolutionAspectRatio.ThirtyNineByEighteen)
                camera.fieldOfView = tall39By18FieldOfView;
        }
        else
        {
            if (aspectRatio == ResolutionAspectRatio.Default)
                camera.fieldOfView = widFieldOfView;
            else if (aspectRatio == ResolutionAspectRatio.FourByThree)
                camera.fieldOfView = wideTabletFieldOfView;
            else if (aspectRatio == ResolutionAspectRatio.ThirtyNineByEighteen)
                camera.fieldOfView = wide39By18FieldOfView;
        }
    }
}
