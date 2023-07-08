using System;
using UnityEngine;

[Serializable]
public sealed class PieceMoved
{
    public MovementType MovementType { get; }
    public int MoveNumber { get; }
    public GameObject Piece { get; }
    public TilePoint From { get; }
    public TilePoint To { get; }
    public TilePoint Delta => To - From;

    public PieceMoved(MovementType moveType, GameObject obj, TilePoint from, TilePoint to, int moveNumber)
    {
        MovementType = moveType;
        MoveNumber = moveNumber;
        Piece = obj;
        From = from;
        To = to;
    }

    public bool HasJumpedOver(GameObject other)
    {
        var hasJumpedOver = From.IsAdjacentTo(new TilePoint(other)) && To.IsAdjacentTo(new TilePoint(other))
                                                       && (To.X == From.X || To.Y == From.Y);
        return hasJumpedOver;
    }

    public bool HasEaten(GameObject other)
    {
        return MovementType == MovementType.Eat && new TilePoint(other).Equals(To);
    }
    
    public void Undo() => Message.Publish(new UndoPieceMoved(MovementType, Piece, From, To, MoveNumber));
}
