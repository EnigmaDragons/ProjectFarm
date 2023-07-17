using UnityEngine;

public class CameraShake : OnMessage<CameraShakeRequested>
{
    private Transform camTransform;
    public float decreaseFactor = 1.0f;

    private float _remainingShakeDuration;
    private float _shakeAmount = 0.7f;
	
    private Vector3 _originalPos;
	
    void Awake()
    {
        if (camTransform == null) 
            camTransform = transform;
    }

    protected override void Execute(CameraShakeRequested msg)
    {
        _originalPos = camTransform.localPosition;
        _remainingShakeDuration = msg.Duration;
        _shakeAmount = msg.Amount;
    }

    void Update()
    {
        if (!(_remainingShakeDuration > 0)) return;
        
        gameObject.transform.localPosition = _originalPos + Random.insideUnitSphere * _shakeAmount;
        _remainingShakeDuration -= Time.deltaTime * decreaseFactor;
        _shakeAmount -= Time.deltaTime * decreaseFactor;
        if (_shakeAmount <= 0)
        {
            _shakeAmount = 0;
            gameObject.transform.localPosition = _originalPos;
        }
    }
}
