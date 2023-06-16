using System;
using UnityEngine;

public class ZoomInAtLevelCompleted : OnMessage<LevelCompleted, LevelReset>
{
    [SerializeField] private float _secondsToZoom;
    [SerializeField] private Vector3 _offset;

    private Camera _camera;
    private bool _zooming;
    private Vector3 _startingPosition;
    private float _t;

    private void Awake()
    {
        _camera = Camera.main;
        _startingPosition = _camera.transform.position;
    }

    protected override void Execute(LevelCompleted msg)
    {
        _zooming = true;
        _startingPosition = _camera.transform.position;
    }

    protected override void Execute(LevelReset msg)
    {
        _zooming = false;
        _camera.gameObject.transform.position = _startingPosition;
    }

    private void Update()
    {
        if (!_zooming)
            return;
        
        Debug.Log("Zooming - Level Complete", this);
        _t = Math.Min(1, _t + Time.deltaTime / _secondsToZoom);
        _camera.transform.position = Vector3.Lerp(_startingPosition, transform.position + _offset, _t);
    }
}
