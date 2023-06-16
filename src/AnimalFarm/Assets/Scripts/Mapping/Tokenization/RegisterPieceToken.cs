using UnityEngine;

public sealed class RegisterPieceToken : MonoBehaviour
{
    [SerializeField] private CurrentMapTokenizer map;
    [SerializeField] private MapPiece piece;

    void Awake()
    {
        map.RegisterAsMapPiece(gameObject, piece);
    }
}
