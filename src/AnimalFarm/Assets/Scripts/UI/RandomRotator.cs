using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    private Vector3 _direction;

    private void Start() => _direction = new Vector3((float)Rng.Dbl() * 2 - 1, (float)Rng.Dbl() * 2 - 1, (float)Rng.Dbl() * 2 - 1);

    private void Update() => transform.Rotate(_direction * speed * Time.deltaTime);
}
