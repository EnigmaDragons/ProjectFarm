using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenContextData
{
    public int KnownMoves;
    public int MaxMoves;
    public Dictionary<TilePoint, MapPiece> Pieces;

    public int MaxRemainingMoves => MaxMoves - KnownMoves;
}

public abstract class MapPieceGenRule
{
    public abstract MapPiece Piece { get; }
    public abstract bool MustPlace(GenContextData ctx);
    public abstract bool CanPlace(GenContextData ctx);
    public abstract bool ShouldPlace(GenContextData ctx);
}

public class TreatPlacementRule : MapPieceGenRule
{
    public override MapPiece Piece => MapPiece.Treat;
    public override bool MustPlace(GenContextData ctx) => ctx.MaxRemainingMoves == 1 && CanPlace(ctx);

    public override bool CanPlace(GenContextData ctx) => !ctx.Pieces.Any(x => x.Value == MapPiece.Treat);
    public override bool ShouldPlace(GenContextData ctx) => CanPlace(ctx) && Rng.Dbl() < Mathf.Clamp(1f / ctx.MaxRemainingMoves, 0f, 1f);
}

public static class LevelGenV1
{
    // TODO: Refactor into rules 
    private static MapPiece SelectNewPathPiece(GenContextData ctx)
    {
        var treatRule = new TreatPlacementRule();
        var hasPlacedDolphin = ctx.Pieces.Any(x => x.Value == MapPiece.Dolphin);
        
        if (treatRule.MustPlace(ctx))
            return treatRule.Piece;

        if (treatRule.ShouldPlace(ctx))
            return treatRule.Piece;
        
        var shouldPlaceDolphin = !hasPlacedDolphin && Rng.Dbl() < 0.1f;
        if (shouldPlaceDolphin)
            return MapPiece.Dolphin;
        
        return MapPiece.Food;
    }

    private static LevelMap GenerateInner(LevelGenV1Params p)
    {
        var maxX = 12;
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
                // Eating Piece - Path Rule
                var movingPiece = MapPiece.HeroAnimal;
                var pathPiece = SelectNewPathPiece(new GenContextData { KnownMoves = knownMoves, MaxMoves = p.MaxMoves, Pieces = pieces });
                var from = heroLoc.Clone();
                var to = possibleHeroAnimalMoves.Random();
                
                lb.MovePieceAndAddFloor(from, to, movingPiece);
                pieces[to] = movingPiece;
                lb.WithPieceAndFloor(from, pathPiece);
                pieces[from] = pathPiece;
                knownMoves++;
            }
            else if (noMovePossible)
            {
                var message = "No move possible. Ending Level Gen.";
                Log.Error(message);
                throw new Exception(message);
            }
            else
            {
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
                    var newPiece = SelectNewPathPiece(new GenContextData{ KnownMoves = knownMoves, MaxMoves = p.MaxMoves, Pieces = pieces });
                    lb.WithPieceAndFloor(tween, newPiece);
                    pieces[tween] = newPiece;
                }
                knownMoves++;
            }

            heroLoc = pieces.Single(x => x.Value == MapPiece.HeroAnimal).Key;
            isFinished = knownMoves >= p.MinMoves && knownMoves <= p.MaxMoves && pieces.Any(p => p.Value == MapPiece.Treat);
            
            if (pieces.Count(x => x.Value == MapPiece.HeroAnimal) > 1)
                Log.Warn("More than 1 Root Key");
            
            if (pieces.Count(x => x.Value == MapPiece.Barn) < 1)
                Log.Warn("Less than 1 Root");
        }

        // Phase 3 - Finalization
        // Rule 3A - Ensure the Genius Path
        
        // Phase 4 - Map Optimization (Trim dead rows/columns)

        // TODO: Implement
        // TODO: Add some random floors?
        
        return lb.Build();
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
