using UnityEngine;

public sealed class InitCurrentSelectedPiece : MonoBehaviour
{
    [SerializeField] private CurrentSelectedPiece piece;

    private void Awake() => piece.Deselect();
    private void OnDisable() => piece.Deselect();
}
