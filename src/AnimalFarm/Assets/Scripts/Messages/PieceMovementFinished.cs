using UnityEngine;

public sealed class PieceMovementFinished
{
    public int MoveNumber { get; }
    public MovementType MovementType { get; }
    public GameObject Object { get; }

    public PieceMovementFinished(MovementType type, GameObject obj, int moveNumber)
    {
        MovementType = type;
        Object = obj;
        MoveNumber = moveNumber;
    }
}
