using UnityEngine;

public class RegisterPiece : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private MapPiece piece;

    private void Awake() => map.Register(gameObject, piece);
}
