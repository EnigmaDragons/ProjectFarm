using System;
using System.Collections.Generic;

public abstract class MapPieceGenRule
{
    public abstract int Priority { get; }
    public abstract MapPiece Piece { get; }
    public abstract bool MustPlace(GenContextData ctx);
    public abstract bool ShouldPlace(GenContextData ctx);
    public abstract void Apply(GenWipData data);
}

public class GenWipData
{
    public LevelMapBuilder Level;
    public Dictionary<TilePoint, MapPiece> Pieces;
    public Action IncrementKnownMoves;
    public TilePoint FromTile;
}
