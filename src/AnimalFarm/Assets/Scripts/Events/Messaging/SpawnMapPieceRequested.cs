
public class SpawnMapPieceRequested
{
    public MapPiece Piece { get; }
    public TilePoint Tile { get; }
    
    public SpawnMapPieceRequested(MapPiece piece, TilePoint tile)
    {
        Piece = piece;
        Tile = tile;
    }
}
