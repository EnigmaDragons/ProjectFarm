using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float speed = 1;
    [SerializeField] private Vector3 direction = new Vector3(1, 0, 0); 

    private void Update() => transform.Rotate(direction * speed * Time.deltaTime);
}
