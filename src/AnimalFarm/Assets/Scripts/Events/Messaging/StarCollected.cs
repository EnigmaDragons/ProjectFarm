using System;
using UnityEngine;

public sealed class StarCollected
{
    public string StarType { get; }
    public Maybe<GameObject> StarPiece { get; }

    public static StarCollected LevelComplete => new StarCollected(nameof(LevelComplete), Maybe<GameObject>.Missing());
    public static StarCollected TreatEaten => new StarCollected(nameof(TreatEaten), Maybe<GameObject>.Missing());
    public static StarCollected AllFoodEaten => new StarCollected(nameof(AllFoodEaten), Maybe<GameObject>.Missing());
    [Obsolete] public static StarCollected OnMapDataCube(GameObject starPiece) => new StarCollected(nameof(OnMapDataCube), starPiece);
    [Obsolete] public static StarCollected NoMoreJumpables => new StarCollected(nameof(NoMoreJumpables), Maybe<GameObject>.Missing());
    [Obsolete] public static StarCollected AllDataNodesRemoved => new StarCollected(nameof(AllDataNodesRemoved), Maybe<GameObject>.Missing());

    public static Maybe<StarCollected> ForCounterType(CounterType counterType)
    {
        if (counterType == CounterType.NumFoodCollected || counterType == CounterType.NumFoodPossible)
            return AllFoodEaten;

        if (counterType == CounterType.NumTreatsCollected || counterType == CounterType.NumTreatsPossible)
            return TreatEaten;
        
        return Maybe<StarCollected>.Missing();
    }

    private StarCollected(string starType, Maybe<GameObject> starPiece)
    {
        StarType = starType;
        StarPiece = starPiece;
    }
    
    public void Undo() => Message.Publish(new UndoStarCollected(StarType, StarPiece));
}
