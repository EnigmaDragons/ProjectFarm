using System;
using UnityEngine;

[Serializable]
public sealed class PieceMoved
{
    public int MoveNumber { get; }
    public GameObject Piece { get; }
    public TilePoint From { get; }
    public TilePoint To { get; }
    public TilePoint Delta => To - From;

    public PieceMoved(GameObject obj, TilePoint from, TilePoint to, int moveNumber)
    {
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

    public void Undo() => Message.Publish(new UndoPieceMoved(Piece, From, To, MoveNumber));
}
