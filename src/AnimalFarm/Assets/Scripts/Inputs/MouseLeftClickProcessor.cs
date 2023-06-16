using UnityEngine;

public class MouseLeftClickProcessor : MonoBehaviour
{
    [SerializeField] private BoolReference gameInputActive;

    private Camera _mainCamera;
    
    void Awake() => _mainCamera = Camera.main;

    void Update () 
    {
        if (!Input.GetMouseButtonDown(0) || !gameInputActive.Value) return;
        
        var rawMousePos = Input.mousePosition;
        var adjustedMousePos = rawMousePos + new Vector3(0, 0, -_mainCamera.transform.position.z);
        var mousePos = _mainCamera.ScreenToWorldPoint(adjustedMousePos);
        var clickedTile = new TilePoint(mousePos);
        Message.Publish(new TileIndicated(clickedTile));
    }
}
