﻿using System.Collections.Generic;
using System.Linq;

public static class GenFunctions
{
    public static readonly HashSet<MapPiece> NpcAnimals = new HashSet<MapPiece>
    {
        MapPiece.Elephant,
        MapPiece.Dolphin,
    };

    public static float AdjustOdds(float baseOdds, MapPiece proposedPiece, HashSet<MapPiece> alreadyPieces)
    {
        var isNpcAnimal = NpcAnimals.Contains(proposedPiece);
        bool HasOtherNpcAnimal() => alreadyPieces.Except(proposedPiece).Any(p => NpcAnimals.Contains(p));
        if (isNpcAnimal && HasOtherNpcAnimal())
            return baseOdds * 0.05f;
        return baseOdds;
    }
}
