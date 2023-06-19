using UnityEngine;

public sealed class PieceJumped
{
    public GameObject Piece { get; }
    public int MoveNumber { get; }
    
    public PieceJumped(GameObject o, int moveNumber)
    {
        Piece = o;
        MoveNumber = moveNumber;
    }
}
