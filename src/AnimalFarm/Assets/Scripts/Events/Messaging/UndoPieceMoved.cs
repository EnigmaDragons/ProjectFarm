using UnityEngine;

public sealed class UndoPieceMoved
{
    public int MoveNumber { get; }
    public GameObject Piece { get; }
    public TilePoint From { get; }
    public TilePoint To { get; }
    public TilePoint Delta => To - From;

    public UndoPieceMoved(GameObject obj, TilePoint from, TilePoint to, int moveNumber)
    {
        Piece = obj;
        From = from;
        To = to;
        MoveNumber = moveNumber;
    }

    public bool HadJumpedOver(GameObject other) => From.IsAdjacentTo(new TilePoint(other)) && To.IsAdjacentTo(new TilePoint(other))
                                                                                           && (To.X == From.X || To.Y == From.Y);
}
