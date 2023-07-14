using System.Linq;

public class DolphinRidePlacementRule : MapPieceGenRule
{
    // TODO: Implement - Check for at least 3 empty spaces in a row
    private bool CanPlace(GenContextData ctx)
    {
        var noDolphinPlacedYet = !ctx.Pieces.Any(x => x.Value == MapPiece.Dolphin);
        return noDolphinPlacedYet;
    }

    public override int Priority => 40;
    public override MapPiece Piece => MapPiece.Dolphin;
    public override bool MustPlace(GenContextData ctx) => ctx.MustInclude.Contains(MapPiece.Dolphin) && ctx.MaxRemainingMoves <= 2 && CanPlace(ctx);
    public override bool ShouldPlace(GenContextData ctx) => CanPlace(ctx) && Rng.Dbl() < GenFunctions.AdjustOdds(0.1f, Piece, ctx.Pieces.Values.ToHashSet());

    public override void Apply(GenWipData data)
    {
        var aborted = false;
        var distance = Rng.Int(3, 8);
        var from = data.FromTile.Clone();
        var movingPiece = data.Pieces[from];
        var to = from.GetAdjacents().Where(x => x.IsInBounds(data.Level.Max) && !data.Pieces.ContainsKey(x) && !data.SpecialFloors.ContainsKey(x)).ToArray().Random();
        // NOTE: Can Adjust Direction after 1 Tile, but then lock it in
        
        var delta = to - from;
        for (var i = 0; i < distance; i++)
        {
            Log.SInfo(LogScopes.Gen, $"Dolphin Path Distance {distance}. Delta/Direction: {delta}");
            data.Level.MovePieceAndAddFloorIfMissing(from, to, movingPiece, MapPiece.Dirt);
            data.Pieces[to] = movingPiece;
            if (i == 0)
            {
                data.Level.WithPieceAndFloor(from, MapPiece.DolphinRideExit, MapPiece.River);
                data.Pieces[from] = MapPiece.DolphinRideExit;
                data.SpecialFloors[from] = MapPiece.River;
            }
            else
            {
                data.Level.WithPieceAndFloor(from, Piece, MapPiece.River);
                data.Pieces[from] = Piece;
                data.SpecialFloors[from] = MapPiece.River;
            }

            var oldFrom = from;
            from = to;
            to = delta + to;
            Log.SInfo(LogScopes.Gen, $"Dolphin Path From {from} -> To {to}");
            if (!to.IsInBounds(data.Level.Max) || data.Pieces.ContainsKey(to) || data.SpecialFloors.ContainsKey(to) || i + 1 == distance)
            {
                if (i == 0)
                {
                    aborted = true;
                    data.Level.WithPieceAndFloor(to, MapPiece.Nothing, MapPiece.Nothing);
                    data.Pieces[to] = MapPiece.Nothing;
                    data.SpecialFloors.Remove(to);
                    data.Level.WithPiece(from, movingPiece);
                    data.Pieces[from] = movingPiece;
                }
                break;
            }

            if (i > 0)
                data.Level.WithPiece(oldFrom, MapPiece.Nothing);
        }

        if (!aborted)
        {
            data.IncrementKnownMoves();
            data.Includes.Add(Piece);
        }
    }
}
