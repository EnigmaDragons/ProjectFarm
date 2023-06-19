using UnityEngine;

public sealed class UndoStarCollected
{
    public string StarType { get; }
    public Maybe<GameObject> StarPiece { get; }

    public UndoStarCollected(string starType, Maybe<GameObject> starPiece)
    {
        StarType = starType;
        StarPiece = starPiece;
    }
}
