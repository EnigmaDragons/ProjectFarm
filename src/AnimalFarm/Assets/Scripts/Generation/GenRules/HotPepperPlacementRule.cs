using System.Linq;

public class HotPepperPlacementRule : MapPieceGenRule
{
    private bool CanPlace(GenContextData ctx) => ctx.Pieces.Count(x => x.Value == Piece) < 2;
    
    public override int Priority => 41;
    public override MapPiece Piece => MapPiece.HotPepper;
    public override bool MustPlace(GenContextData ctx) => ctx.MustInclude.Contains(MapPiece.Dolphin) && ctx.MaxRemainingMoves <= 2 && CanPlace(ctx);
    public override bool ShouldPlace(GenContextData ctx) => CanPlace(ctx) && Rng.Dbl() < GenFunctions.AdjustOdds(0.25f, Piece, ctx.Pieces.Values.ToHashSet(), ctx.MustInclude);
    
    public override void Apply(GenWipData data)
    {
        var aborted = false;
        var distance = 1 + 3;
        var from = data.FromTile.Clone();
        var movingPiece = data.Pieces[from];
        var to = from.GetAdjacents().Where(x => x.CanHavePlacedPiece(data)).ToArray().Random();
        
        var delta = to - from;
        for (var i = 0; i < distance; i++)
        {
            Log.SInfo(LogScopes.Gen, $"Hot Pepper. Delta/Direction: {delta}");
            data.Level.WithMovedPieceAndAddedFloorIfMissing(from, to, movingPiece, MapPiece.Dirt);
            data.Pieces[to] = movingPiece;
            data.Level.WithPieceAndFloor(from, i == distance - 1 ? Piece : MapPiece.Nothing, MapPiece.Dirt);
            data.Pieces[from] = Piece;

            var oldFrom = from;
            from = to;
            to = delta + to;
            Log.SInfo(LogScopes.Gen, $"Hot Pepper Path From {from} -> To {to}");
            if (!to.IsInBounds(data.Level.Max) || data.Pieces.ContainsKey(to) || data.SpecialFloors.ContainsKey(to) && !data.SpecialFloors[to].Rules().IsWalkable || i + 1 == distance)
            {
                if (i < distance - 1)
                {
                    aborted = true;
                    for (var j = 0; j < i; j++)
                    {
                        data.Level.WithPieceAndFloor(oldFrom, MapPiece.Nothing, MapPiece.Nothing);
                        data.Pieces[oldFrom] = MapPiece.Nothing;
                        data.SpecialFloors.Remove(oldFrom);
                        data.Level.WithPiece(from, movingPiece);
                        data.Pieces[from] = movingPiece;
                        oldFrom = from;
                        from = to;
                        to = delta + to;
                    }
                }
                break;
            }
        }

        if (!aborted)
        {
            data.IncrementKnownMoves();
            data.Includes.Add(Piece);
        }
    }
}
