using System.Linq;

public class ElephantPlacementRule : MapPieceGenRule
{
    private bool CanPlace(GenContextData ctx) => ctx.Pieces.Count(x => x.Value == MapPiece.Food) > 2 && !ctx.Pieces.Any(x => x.Value == MapPiece.Elephant);
    public override int Priority => 41;
    public override MapPiece Piece => MapPiece.Elephant;
    
    public override bool MustPlace(GenContextData ctx) => ctx.MustInclude.Contains(MapPiece.Elephant) && ctx.MaxRemainingMoves <= 3 && CanPlace(ctx);
    public override bool ShouldPlace(GenContextData ctx) => CanPlace(ctx) && Rng.Dbl() < 0.25f;

    public override void Apply(GenWipData data)
    {
        var from = data.FromTile.Clone();
        var targetPos = from.GetAdjacents().Where(x => x.IsInBounds(data.Level.MaxX, data.Level.MaxY) && !data.Pieces.ContainsKey(x)).ToArray().Random();

        data.Level.WithPieceAndFloor(targetPos, Piece);
        data.Pieces[targetPos] = Piece;
        var numFoodPlanted = 0;
        foreach (var piece in data.Pieces.Where(p => p.Value == MapPiece.Food))
        {
            numFoodPlanted++;
            var foodTile = piece.Key;
            data.Level.WithPieceAndFloor(foodTile, MapPiece.Nothing, MapPiece.Seedling);
        }
        
        Log.SInfo(LogScopes.Gen, $"Planted {numFoodPlanted} seedlings");
        Log.SInfo(LogScopes.Gen, $"Placed {Piece} at {from}");
        data.IncrementKnownMoves();
    }
}
