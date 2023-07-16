using System.Collections.Generic;
using System.Linq;

public static class GenFunctions
{
    public static readonly HashSet<MapPiece> NpcAnimals = new HashSet<MapPiece>
    {
        MapPiece.Elephant,
        MapPiece.Dolphin,
        MapPiece.Dino,
    };

    public static float AdjustOdds(float baseOdds, MapPiece proposedPiece, HashSet<MapPiece> alreadyPieces, HashSet<MapPiece> mustIncludes)
    {
        var isNpcAnimal = NpcAnimals.Contains(proposedPiece);
        bool HasOtherNpcAnimal() => alreadyPieces.Except(proposedPiece).Concat(mustIncludes).Any(p => NpcAnimals.Contains(p));
        if (isNpcAnimal && HasOtherNpcAnimal())
            return baseOdds * 0.05f;
        return baseOdds;
    }

    public static bool CanHavePlacedPiece(this TilePoint tp, GenWipData data)
    {
        return tp.IsInBounds(data.Level.MaxX, data.Level.MaxY)
               && !data.Pieces.ContainsKey(tp)
               && (!data.SpecialFloors.ContainsKey(tp) || data.SpecialFloors[tp].Rules().IsWalkable);
    }
}
