using UnityEngine;

public sealed class MoveToRequested
{
    public GameObject Piece { get; }
    public TilePoint From { get; }
    public TilePoint To { get; }
    public TilePoint Delta => To - From;

    public MoveToRequested(GameObject obj, TilePoint from, TilePoint to)
    {
        Piece = obj;
        From = from;
        To = to;
    }
}
