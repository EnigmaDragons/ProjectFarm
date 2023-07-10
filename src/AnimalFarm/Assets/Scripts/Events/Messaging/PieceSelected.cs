using UnityEngine;

public sealed class PieceSelected
{
    public GameObject Piece { get; }
    public bool SuppressSound { get; }

    public PieceSelected(GameObject o, bool suppressSound = false)
    {
        Piece = o;
        SuppressSound = suppressSound;
    }
}
