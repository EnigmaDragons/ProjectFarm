using System.Linq;

public class ElephantPlacementRule : MapPieceGenRule
{
    private bool CanPlace(GenContextData ctx) => ctx.Pieces.Count(x => x.Value == MapPiece.Food) > 2 && !ctx.Pieces.Any(x => x.Value == Piece);
    public override int Priority => 41;
    public override MapPiece Piece => MapPiece.Elephant;
    
    public override bool MustPlace(GenContextData ctx) => ctx.MustInclude.Contains(MapPiece.Elephant) && ctx.MaxRemainingMoves <= 3 && CanPlace(ctx);
    public override bool ShouldPlace(GenContextData ctx) => CanPlace(ctx) && Rng.Dbl() < GenFunctions.AdjustOdds(0.25f, Piece, ctx.Pieces.Values.ToHashSet(), ctx.MustInclude);

    public override void Apply(GenWipData data)
    {
        var from = data.FromTile.Clone();
        var targetPos = from.GetAdjacents().Where(x => x.CanHavePlacedPiece(data)).ToArray().Random();

        data.Level.WithPieceAndFloor(targetPos, Piece, MapPiece.Dirt);
        data.Pieces[targetPos] = Piece;
        Log.SInfo(LogScopes.Gen, $"Placed {Piece} at {targetPos}");
        
        var numFoodPlanted = 0;
        var totalFood = data.Pieces.Count(x => x.Value == MapPiece.Food);
        var numSpecialFloors = data.SpecialFloors.Count;
        var foodToPlant = data.Pieces.Where(p => p.Value == MapPiece.Food && !data.SpecialFloors.ContainsKey(p.Key)).ToArray();
        foreach (var piece in foodToPlant)
        {
            numFoodPlanted++;
            var foodTile = piece.Key;
            data.Level.WithPieceAndFloor(foodTile, MapPiece.Nothing, MapPiece.Seedling);
            data.Pieces.Remove(foodTile);
            data.SpecialFloors[foodTile] = MapPiece.Seedling;
            Log.SInfo(LogScopes.Gen, $"Planted seedling at {foodTile}");
        }
        
        Log.SInfo(LogScopes.Gen, $"Planted {numFoodPlanted} seedlings out of {totalFood} food. Num Previous Special Floors: {numSpecialFloors}");
        data.IncrementKnownMoves();
        data.Includes.Add(Piece);
    }
}
