using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenContextData
{
    public int KnownMoves;
    public int MaxMoves;
    public Dictionary<TilePoint, MapPiece> Pieces;
    public HashSet<MapPiece> MustInclude = new HashSet<MapPiece>();

    public int MaxRemainingMoves => MaxMoves - KnownMoves;
}

public class GenWipData
{
    public LevelMapBuilder Level;
    public Dictionary<TilePoint, MapPiece> Pieces;
    public Action IncrementKnownMoves;
    public TilePoint FromTile;
}

public abstract class MapPieceGenRule
{
    public abstract int Priority { get; }
    public abstract MapPiece Piece { get; }
    public abstract bool MustPlace(GenContextData ctx);
    public abstract bool ShouldPlace(GenContextData ctx);
    public abstract void Apply(GenWipData data);
}

public class TreatPlacementRule : MapPieceGenRule
{
    private bool CanPlace(GenContextData ctx) => !ctx.Pieces.Any(x => x.Value == MapPiece.Treat);
    
    public override int Priority => 1;
    public override MapPiece Piece => MapPiece.Treat;
    public override bool MustPlace(GenContextData ctx) => ctx.MaxRemainingMoves == 1 && CanPlace(ctx);
    public override bool ShouldPlace(GenContextData ctx) => CanPlace(ctx) && Rng.Dbl() < Mathf.Clamp(1f / ctx.MaxRemainingMoves, 0f, 1f);
    
    public override void Apply(GenWipData data) => FoodPlacementRule.Apply(data, Piece);
}

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
                
        data.Level.MovePieceAndAddFloor(from, to, movingPiece);
        data.Pieces[to] = movingPiece;
        data.Level.WithPieceAndFloor(from, piece);
        data.Pieces[from] = piece;
        Log.SInfo(LogScopes.Gen, $"Moved {movingPiece} to {to}");
        Log.SInfo(LogScopes.Gen, $"Placed {piece} at {from}");
        data.IncrementKnownMoves();
    }
}

public static class LevelGenV1
{
    private static MapPieceGenRule SelectNewPathPiece(GenContextData ctx)
    {
        var rules = new MapPieceGenRule[]
        {
            new TreatPlacementRule(),
            new DolphinRidePlacementRule(),
            new FoodPlacementRule()
        }.OrderBy(r => r.Priority).ToArray();

        foreach (var rule in rules)
            if (rule.MustPlace(ctx))
                return rule;
        
        foreach (var rule in rules)
            if (rule.ShouldPlace(ctx))
                return rule;

        return new FoodPlacementRule();
    }

    private static LevelMap GenerateInner(LevelGenV1Params p)
    {
        var mustIncludes = new HashSet<MapPiece>(p.MustInclude);
        var maxX = 10;
        var maxY = 7;
        var lb = new LevelMapBuilder(Guid.NewGuid().ToString(), maxX, maxY);

        var pieces = new Dictionary<TilePoint, MapPiece>();
        
        // Phase 1 - Initial Setup
        // Rule 1A - Add a Barn
        var barnLoc = new TilePoint(Rng.Int(0, maxX), Rng.Int(0, maxY));
        lb.WithPieceAndFloor(barnLoc, MapPiece.Barn);
        pieces[barnLoc] = MapPiece.Barn;
        
        // Rule 1B - Place a Hero Animal
        var adjacents = barnLoc.GetAdjacents().Where(x => x.IsInBounds(maxX, maxY)).ToArray();
        var heroLoc = adjacents.Random();
        lb.WithPieceAndFloor(heroLoc, MapPiece.HeroAnimal);
        pieces[heroLoc] = MapPiece.HeroAnimal;
        heroLoc = pieces.Single(x => x.Value == MapPiece.HeroAnimal).Key;
        
        // Phase 2 - Puzzle Meat
        // Rule 2A - Generate A Food Path 
        // Rule 2B - Place a Star Food
        // Rule 2C - Place a Dolphin & Path
        var knownMoves = 0;
        var isFinished = false;
        var piecesWhoCannotMove = new HashSet<TilePoint>();

        while (!isFinished)
        {
            var nonHeroSelectablePieces = pieces
                    .Where(x => x.Value.Rules().IsSelectable && x.Value != MapPiece.HeroAnimal).ToArray();
            var possibleHeroAnimalMoves = heroLoc.GetAdjacents().Where(x => x.IsInBounds(maxX, maxY) && !pieces.ContainsKey(x)).ToArray();
            var heroAnimalCanMove = possibleHeroAnimalMoves.Length > 0;
            var shouldMoveHeroAnimal = heroAnimalCanMove && nonHeroSelectablePieces.Length == 0 || Rng.Dbl() < 0.6;
            var noMovePossible = !heroAnimalCanMove && nonHeroSelectablePieces.Length == 0;
            if (shouldMoveHeroAnimal)
            {
                var pieceRule = SelectNewPathPiece(new GenContextData { KnownMoves = knownMoves, MaxMoves = p.MaxMoves, Pieces = pieces, MustInclude = mustIncludes });
                var data = new GenWipData
                {
                    Level = lb,
                    Pieces = pieces,
                    FromTile = heroLoc,
                    IncrementKnownMoves = () =>
                    {
                        knownMoves += 1;
                    }
                };
                pieceRule.Apply(data);
            }
            else if (noMovePossible)
            {
                var message = "No move possible. Ending Level Gen.";
                Log.Error(message);
                throw new Exception(message);
            }
            else
            {
                Log.Warn($"Should not be hitting this branch, unless another Piece Type is Selectable. Selectable Pieces are {string.Join(", ", nonHeroSelectablePieces.Select(x => x.Value))}");
                
                // Jumping Piece - Path Rule
                piecesWhoCannotMove.Clear();
                var toOptions = Array.Empty<(TilePoint t, List<TilePoint> tp)>();
                var movingPieceEntry = nonHeroSelectablePieces.First();
                
                for (var i = 0; toOptions.Length < 1 || i < p.MaxConsecutiveMisses; i++)
                {
                    movingPieceEntry = nonHeroSelectablePieces.Where(x => !piecesWhoCannotMove.Contains(x.Key)).Random();
                    var from = movingPieceEntry.Key;
                    toOptions = from.GetCardinals(2)
                        .Select(t => (t, t.InBetween(from)))
                        .Where(d => d.t.IsInBounds(maxX, maxY)
                                    && !pieces.ContainsKey(d.t)
                                    && d.Item2.Any(tweenTile => !pieces.ContainsKey(tweenTile)))
                        .ToArray();
                    if (toOptions.Length == 0)
                    {
                        Log.Warn("Skipping a Cycle. Picked an impossible Selectable piece to move");
                        piecesWhoCannotMove.Add(movingPieceEntry.Key);
                    }
                }

                if (toOptions.Length == 0)
                {
                    throw new Exception($"Fatal Gen Exception: Could not find a valid move for a Selectable piece within {p.MaxConsecutiveMisses} Tries.");
                }

                var option = toOptions.Random();
                var to = option.t;
                var movingPiece = movingPieceEntry.Value;
                var tweens = option.Item2;
                
                lb.MovePieceAndAddFloor(movingPieceEntry.Key, to, movingPiece);
                pieces[to] = movingPiece;
                
                foreach (var tween in tweens)
                {
                    var newPieceRule = SelectNewPathPiece(new GenContextData { KnownMoves = knownMoves, MaxMoves = p.MaxMoves, Pieces = pieces, MustInclude = mustIncludes });
                    lb.WithPieceAndFloor(tween, newPieceRule.Piece);
                    pieces[tween] = newPieceRule.Piece;
                }
                knownMoves++;
            }

            heroLoc = pieces.Single(x => x.Value == MapPiece.HeroAnimal).Key;
            isFinished = knownMoves >= p.MinMoves 
                         && knownMoves <= p.MaxMoves 
                         && pieces.Any(piece => piece.Value == MapPiece.Treat) 
                         && mustIncludes.All(i => pieces.Any(piece => piece.Value == i));
            
            if (pieces.Count(x => x.Value == MapPiece.HeroAnimal) > 1)
                Log.Warn("More than 1 Root Key");
            
            if (pieces.Count(x => x.Value == MapPiece.Barn) < 1)
                Log.Warn("Less than 1 Root");
        }

        // Phase 3 - Finalization
        // Rule 3A - Ensure the Genius Path
        // TODO: Add some random floors?
        
        // Phase 4 - Map Optimization (Trim dead rows/columns)
        // Rule 4A - Flip X/Y if Taller than Wide
        
        var level = FlipXyIfTallerThanWide(lb);
        return level;
    }

    private static LevelMap FlipXyIfTallerThanWide(LevelMapBuilder lb)
    {
        var level = lb.Build();
        var finalMinX = 99;
        var finalMaxX = 0;
        var finalMinY = 99;
        var finalMaxY = 0;
        level.GetIterator().ForEach(t =>
        {
            var (x, y) = t;
            if (level.FloorLayer[x, y] == MapPiece.Nothing)
                return;
            
            finalMinX = Math.Min(finalMinX, x);
            finalMaxX = Math.Max(finalMaxX, x);
            finalMinY = Math.Min(finalMinY, y);
            finalMaxY = Math.Max(finalMaxY, y);
        });
        var finalWidth = (finalMaxX - finalMinX) + 1;
        var finalHeight = (finalMaxY - finalMinY) + 1;
        Log.SInfo(LogScopes.Gen, $"Pre-Flip Final Level Effective Size: {finalWidth}x{finalHeight}. X:{finalMinX}-{finalMaxX}. Y:{finalMinY}-{finalMaxY}");
        return (finalWidth < finalHeight) ? level.GetWithFlippedXY() : level;
    }

    public static LevelMap Generate(LevelGenV1Params p)
    {
        var permitted = p.Validate();
        if (!permitted)
        {
            var msg = $"Invalid Gen Params: {permitted}";
            Log.Error(msg);
            throw new Exception(msg);
        }

        var maxAttempts = p.MaxNumGenRetries + 1;
        for (var i = 0; i < maxAttempts; i++)
        {
            var attemptNumber = i + 1;
            
            try
            {
                return GenerateInner(p);
            }
            catch (Exception e)
            {
                Log.SInfo(LogScopes.Gen, string.Format("Failed to Generate Map. Retrying... Attempt {0} of {1}", attemptNumber, maxAttempts));
            }
        }
        
        throw new Exception($"Failed to Generate Map after {maxAttempts} attempts.");
    }
}
