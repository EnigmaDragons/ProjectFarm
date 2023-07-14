using System.Linq;

public class FoodPlacementRule : MapPieceGenRule
{
    public override int Priority => 99;
    public override MapPiece Piece => MapPiece.Food;
    public override bool MustPlace(GenContextData ctx) => false;
    public override bool ShouldPlace(GenContextData ctx) => true;
    
    public override void Apply(GenWipData data) => Apply(data, Piece);

    public static void Apply(GenWipData data, MapPiece piece)
    {
        var from = data.FromTile.Clone();
        var to = from.GetAdjacents().Where(x => x.IsInBounds(data.Level.MaxX, data.Level.MaxY) && !data.Pieces.ContainsKey(x)).ToArray().Random();
        var movingPiece = data.Pieces[from];
                
        data.Level.MovePieceAndAddFloorIfMissing(from, to, movingPiece, MapPiece.Dirt);
        data.Pieces[to] = movingPiece;
        Log.SInfo(LogScopes.Gen, $"Moved {movingPiece} to {to}");
        
        data.Level.WithPiece(from, piece);
        data.Pieces[from] = piece;
        Log.SInfo(LogScopes.Gen, $"Placed {piece} at {from}");
        
        data.IncrementKnownMoves();
    }
}
