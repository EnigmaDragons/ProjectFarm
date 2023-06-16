using UnityEngine;

public class MovementProposed
{
    public MovementType Type { get; }
    public GameObject Piece { get; }
    public TilePoint From { get; }
    public TilePoint To { get; }
    public TilePoint Delta => To - From;

    public MovementProposed(MovementType type, GameObject obj, TilePoint from, TilePoint to)
    {
        Type = type;
        Piece = obj;
        From = from;
        To = to;
    }
}
