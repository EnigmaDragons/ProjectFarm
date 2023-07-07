using DG.Tweening;
using UnityEngine;

public sealed class ZoomInOnDestinationAtLevelEnd : OnMessage<LevelCompleted>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private FloatReference zoomDuration = new FloatReference(1);

    private Camera _camera;
    
    private void Awake() => _camera = Camera.main;
    
    protected override void Execute(LevelCompleted msg)
    {
        Debug.Log("Zooming - Level Complete", this);
        _camera.transform.DOMove(map.FinalCameraAngle.position, zoomDuration);
        _camera.transform.DORotateQuaternion(map.FinalCameraAngle.rotation, zoomDuration);
    }
}
