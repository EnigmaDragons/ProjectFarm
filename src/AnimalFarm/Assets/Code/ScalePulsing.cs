using UnityEngine;

public sealed class ScalePulsing : MonoBehaviour
{
    [SerializeField] private Vector3 varyAmount = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private float speed = 10;

    private Vector3 _min;
    private Vector3 _max;
    private int _direction = 1;
    private float _current = 0.5f;
    
    private void Awake()
    {
        _min = transform.localScale - varyAmount;
        _max = transform.localScale + varyAmount;
    }

    private void Update()
    {
        _current = Mathf.Max(0, Mathf.Min(1, _current + Time.deltaTime * speed * _direction));
        if (_current >= 1f && _direction > 0 || _current <= 0 && _direction < 0)
            _direction = _direction * -1;

        transform.localScale = Vector3.Lerp(_min, _max, _current);
    }
}
