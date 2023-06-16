using UnityEngine;

public sealed class PieceSelected
{
    public GameObject Piece { get; }

    public PieceSelected(GameObject o) => Piece = o;
}
