using UnityEngine;

public sealed class StarCollected
{
    public string StarType { get; }
    public Maybe<GameObject> StarPiece { get; }

    public static StarCollected LevelComplete => new StarCollected(nameof(LevelComplete), Maybe<GameObject>.Missing());
    public static StarCollected OnMapDataCube(GameObject starPiece) => new StarCollected(nameof(OnMapDataCube), starPiece);
    public static StarCollected NoMoreJumpables => new StarCollected(nameof(NoMoreJumpables), Maybe<GameObject>.Missing());
    public static StarCollected AllDataNodesRemoved => new StarCollected(nameof(AllDataNodesRemoved), Maybe<GameObject>.Missing());

    private StarCollected(string starType, Maybe<GameObject> starPiece)
    {
        StarType = starType;
        StarPiece = starPiece;
    }
    
    public void Undo() => Message.Publish(new UndoStarCollected(StarType, StarPiece));
}
