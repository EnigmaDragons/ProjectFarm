using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class CameraHighlighter : OnMessage<BeginCameraHighlight, EndCameraHighlight>
{
    [SerializeField] private Vector3 highlightOffset = new Vector3(0.5f, 2.5f, -0.5f);
    [SerializeField] private Vector3 highlightRotation = new Vector3(45, 0, 0);
    [SerializeField] private FloatReference durationIn = new FloatReference(0.3f);
    [SerializeField] private FloatReference durationOut = new FloatReference(0.3f);

    private GameObject[] _targets = Array.Empty<GameObject>();
    private Vector3 _originalPos;
    private Vector3 _originalRot;
    private bool _initialInFinished;
    private Vector3 _additionalOffset = Vector3.zero;
    
    protected override void Execute(BeginCameraHighlight msg)
    {
        var t = transform;
        _originalPos = t.localPosition;
        _originalRot = t.eulerAngles;
        _targets = msg.Targets;
        _initialInFinished = false;
        _additionalOffset = msg.AdditionalOffset;

        t.DOLocalRotate(highlightRotation, durationIn);
        var focalCenter = GetFocalCenter();
        var focalTarget = focalCenter + highlightOffset + _additionalOffset;
        t.DOLocalMove(focalTarget, durationIn);
        Log.SInfo(LogScopes.Camera, $"Focusing at {focalTarget}, based on {msg.Targets.Length} targets. [{string.Join(", ", msg.Targets.Select(t2 => t2.transform.position.ToString()))}]. Bound Centers: {focalCenter}. Highlight Offset: {highlightOffset}");
        
        StartCoroutine(FinishInitialIn());
    }

    protected override void Execute(EndCameraHighlight msg)
    {
        var t = transform;
        t.DOLocalMove(_originalPos, durationOut);
        t.DOLocalRotate(_originalRot, durationOut);
        _targets = Array.Empty<GameObject>();
    }

    private void LateUpdate()
    {
        if (_targets.Length == 0 || !_initialInFinished)
            return;
        
        var center = GetFocalCenter();
        transform.localPosition = center + highlightOffset + _additionalOffset;
    }
    
    private Vector3 GetFocalCenter()
    {
        if (_targets.Length == 0)
            return Vector3.zero;
        
        var bounds = new Bounds(_targets[0].transform.position, Vector3.zero);
        foreach (var target in _targets)
        {
            bounds.Encapsulate(target.transform.position);
            bounds.Encapsulate(target.transform.position + new Vector3(1, 0, 1));
        }

        var center = bounds.center;
        return center;
    }

    private IEnumerator FinishInitialIn()
    {
        yield return new WaitForSeconds(durationIn);
        _initialInFinished = true;
    }
}
