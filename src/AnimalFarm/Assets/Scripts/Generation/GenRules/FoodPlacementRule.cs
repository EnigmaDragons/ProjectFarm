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
        var movingPiece = data.Pieces[from];
        var options = from.GetAdjacents().Where(x => x.CanHavePlacedPiece(data)).ToArray();
        if (options.Length == 0)
        {
            Log.SInfo(LogScopes.Gen, $"Unable to place {piece} adjacent to {from}");
            return;
        }
        var to = options.Random();
                
        data.Level.WithMovedPieceAndAddedFloorIfMissing(from, to, movingPiece, MapPiece.Dirt);
        data.Pieces[to] = movingPiece;
        Log.SInfo(LogScopes.Gen, $"Moved {movingPiece} to {to}");
        
        data.Level.WithPiece(from, piece);
        data.Pieces[from] = piece;
        Log.SInfo(LogScopes.Gen, $"Placed {piece} at {from}");
        
        data.IncrementKnownMoves();
        data.Includes.Add(piece);
    }
}
