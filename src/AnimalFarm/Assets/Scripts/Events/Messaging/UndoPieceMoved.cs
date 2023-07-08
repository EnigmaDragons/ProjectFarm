using UnityEngine;

public sealed class UndoPieceMoved
{
    public int MoveNumber { get; }
    public MovementType MovementType { get; }
    public GameObject Piece { get; }
    public TilePoint From { get; }
    public TilePoint To { get; }
    public TilePoint Delta => To - From;

    public UndoPieceMoved(MovementType movementType, GameObject obj, TilePoint from, TilePoint to, int moveNumber)
    {
        MovementType = movementType;
        Piece = obj;
        From = from;
        To = to;
        MoveNumber = moveNumber;
    }

    public bool HadJumpedOver(GameObject other) => From.IsAdjacentTo(new TilePoint(other)) && To.IsAdjacentTo(new TilePoint(other))
                                                                                           && (To.X == From.X || To.Y == From.Y);

    public bool HadEaten(GameObject other) => MovementType == MovementType.Eat && From.IsAdjacentTo(new TilePoint(other)) && To.Equals(new TilePoint(other));
}
