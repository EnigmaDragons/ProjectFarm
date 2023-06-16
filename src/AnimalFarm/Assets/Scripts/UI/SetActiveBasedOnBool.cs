using UnityEngine;

public class SetActiveBasedOnBool : MonoBehaviour
{
    [SerializeField] private BoolReference boolRef;
    [SerializeField] private bool _inverted;

    private void Start() => gameObject.SetActive(_inverted ? !boolRef.Value : boolRef.Value);
}
