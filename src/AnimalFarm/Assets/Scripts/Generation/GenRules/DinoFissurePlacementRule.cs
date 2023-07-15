using System.Linq;
using UnityEngine;

public class DinoFissurePlacementRule : MapPieceGenRule
{
    private bool CanPlace(GenContextData ctx) => ctx.Pieces.Count > 5 && !ctx.Pieces.Any(x => x.Value == Piece);
    
    public override int Priority => 40;
    public override MapPiece Piece => MapPiece.Dino;

    public override bool MustPlace(GenContextData ctx) => ctx.MustInclude.Contains(Piece) && ctx.MaxRemainingMoves <= 3 && CanPlace(ctx);
    public override bool ShouldPlace(GenContextData ctx) => CanPlace(ctx) && Rng.Dbl() < GenFunctions.AdjustOdds(0.25f, Piece, ctx.Pieces.Values.ToHashSet());

    public override void Apply(GenWipData data)
    {
        var from = data.FromTile.Clone();
        var targetPos = from.GetAdjacents().Where(x => x.IsInBounds(data.Level.MaxX, data.Level.MaxY) && !data.Pieces.ContainsKey(x)).ToArray().Random();
        
        var possibleX = Enumerable.Range(data.Level.EffectiveMinX + 1, data.Level.EffectiveWidth - 2)
            .Where(x => x != from.X && x != targetPos.X).ToArray();
        var possibleY = Enumerable.Range(data.Level.EffectiveMinY + 1, data.Level.EffectiveHeight - 2)
            .Where(y => y != from.Y && y != targetPos.Y).ToArray();

        if (!possibleX.Any() && !possibleY.Any())
        {
            Log.SInfo(LogScopes.Gen, $"Aborting Dino Fissure due to no valid options.");
            return;
        }
        
        // NOTE: Place Dino
        data.Level.WithPieceAndFloor(targetPos, MapPiece.Dino, MapPiece.Dirt);
        data.Pieces[targetPos] = MapPiece.Dino;
        Log.SInfo(LogScopes.Gen, $"Placed {Piece} at {targetPos}");
        
        // NOTE: Select Fissure
        var options = possibleX.Select(x => new Vector2Int(x, -1)).Concat(possibleY.Select(y => new Vector2Int(-1, y))).ToArray();
        Log.SInfo(LogScopes.Gen, $"Fissure Options - ${string.Join(",", options.Select(xy => xy.ToString()))}");
        var selectedFissure = options.Random();
        
        // NOTE: Select Shift Direction
        var fissureOffset = new Vector2Int(); 
        if (selectedFissure.x == -1)
        {
            var fissurePositiveRank = selectedFissure.y + 1;
            fissureOffset = (fissurePositiveRank != from.Y && fissurePositiveRank != targetPos.Y)
                ? new Vector2Int(0, 1)
                : new Vector2Int(0, -1);
        }
        if (selectedFissure.y == -1)
        {
            var fissurePositiveFile = selectedFissure.x + 1;
            fissureOffset = (fissurePositiveFile != from.X && fissurePositiveFile != targetPos.X)
                ? new Vector2Int(1, 0)
                : new Vector2Int(-1, 0);
        }
        
        var fissureOffsetTile = new TilePoint(fissureOffset.x, fissureOffset.y);
        bool WillBeInBounds(TilePoint xy) =>
            xy.Plus(fissureOffsetTile).IsInBounds(data.Level.Max);

        if (selectedFissure.x > -1) // Column Fissure
        {
            if (fissureOffset.x > 0) // Right
            {
                for (var x = data.Level.EffectiveMaxX; x >= selectedFissure.x; x--)
                {
                    var xInner = x;
                    data.Level.WithShifted(xy => xy.X == xInner && WillBeInBounds(xy), fissureOffset);
                    ShiftStateColumn(data, x, fissureOffsetTile);
                }
            }

            if (fissureOffset.x < 0) // Left
            {
                for (var x = data.Level.EffectiveMinX; x <= selectedFissure.x; x++)
                {
                    var xInner = x;
                    data.Level.WithShifted(xy => xy.X == xInner && WillBeInBounds(xy), fissureOffset);
                    ShiftStateColumn(data, x, fissureOffsetTile);
                }
            }

            for (var y = 0; y < data.Level.MaxY; y++)
            {
                var fissureTile = new TilePoint(selectedFissure.x, y);
                data.Level.WithFloor(fissureTile, MapPiece.Fissure);
                data.SpecialFloors[fissureTile] = MapPiece.Fissure;
                Log.SInfo(LogScopes.Gen, $"Placed {MapPiece.Fissure} at {fissureTile}");
            }
        }

        if (selectedFissure.y > -1) // Row Fissure
        {
            if (fissureOffset.y > 0) // Up
            {
                for (var y = data.Level.EffectiveMaxY; y >= selectedFissure.y; y--)
                {
                    var yInner = y;
                    data.Level.WithShifted(xy => xy.Y == yInner && WillBeInBounds(xy), fissureOffset);
                    ShiftStateRow(data, y, fissureOffsetTile);
                }
            }
            if (fissureOffset.y < 0) // Down
            {
                for (var y = data.Level.EffectiveMinY; y <= selectedFissure.y; y++)
                {
                    var yInner = y;
                    data.Level.WithShifted(xy => xy.Y == yInner && WillBeInBounds(xy), fissureOffset);
                    ShiftStateRow(data, y, fissureOffsetTile);
                }
            }
            for (var x = 0; x < data.Level.MaxX; x++)
            {
                var fissureTile = new TilePoint(x, selectedFissure.y);
                data.Level.WithFloor(fissureTile, MapPiece.Fissure);
                data.SpecialFloors[fissureTile] = MapPiece.Fissure;
                Log.SInfo(LogScopes.Gen, $"Placed {MapPiece.Fissure} at {fissureTile}");
            }
        }

        Log.SInfo(LogScopes.Gen, $"Shifted {selectedFissure} by {fissureOffset}");
    }

    private static void ShiftStateColumn(GenWipData data, int x, TilePoint fissureOffsetTile)
    {
        for (var y = 0; y < data.Level.MaxY; y++)
        {
            var from = new TilePoint(x, y);
            var to = from + fissureOffsetTile;
            if (data.Pieces.TryGetValue(from, out var piece))
                data.Pieces[to] = piece;
            if (data.SpecialFloors.TryGetValue(from, out var floor))
                data.SpecialFloors[to] = floor;
            data.Pieces.Remove(from);
            data.SpecialFloors.Remove(from);
        }
    }
    
    private static void ShiftStateRow(GenWipData data, int y, TilePoint fissureOffsetTile)
    {
        for (var x = 0; x < data.Level.MaxX; x++)
        {
            var from = new TilePoint(x, y);
            var to = from + fissureOffsetTile;
            if (data.Pieces.TryGetValue(from, out var piece))
                data.Pieces[to] = piece;
            if (data.SpecialFloors.TryGetValue(from, out var floor))
                data.SpecialFloors[to] = floor;
            data.Pieces.Remove(from);
            data.SpecialFloors.Remove(from);
        }
    }
}
