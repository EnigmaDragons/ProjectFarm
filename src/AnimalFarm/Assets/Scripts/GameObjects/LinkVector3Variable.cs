using UnityEngine;

public class LinkVector3Variable : MonoBehaviour
{
    [SerializeField] private Vector3Variable variable;

    private void Update() => variable.Value = transform.position;
}
