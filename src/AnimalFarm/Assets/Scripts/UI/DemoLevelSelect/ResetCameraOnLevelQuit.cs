using UnityEngine;

public sealed class ResetCameraOnLevelQuit : OnMessage<DemoQuitLevelRequested>
{
    private Camera _camera;
    private Vector3 _startingPosition;

    private void Awake()
    {
        _camera = Camera.main;
        _startingPosition = _camera.transform.position;
    }
    
    protected override void Execute(DemoQuitLevelRequested msg)
    {
        _camera.transform.position = _startingPosition;
    }
}
