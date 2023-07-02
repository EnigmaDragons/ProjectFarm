using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelGenV1
{
    private static MapPiece SelectNewPathPiece(int knownMoves, int maxMoves, Dictionary<TilePoint, MapPiece> pieces)
    {
        var hasPlacedStarFood = pieces.Any(x => x.Value == MapPiece.StarFood);
        var maxRemainingMoves = maxMoves - knownMoves;
        var pathPiece = !hasPlacedStarFood && Rng.Dbl() < Mathf.Clamp(1f / maxRemainingMoves, 0f, 1f)
            ? MapPiece.StarFood
            : MapPiece.Food;
        return pathPiece;
    }
    
    public static LevelMap Generate(LevelGenV1Params p)
    {
        var maxX = 7;
        var maxY = 12;
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

        while (!isFinished)
        {
            var shouldMoveHeroAnimal = pieces.Count < 2 || Rng.Dbl() < 0.6;
            if (shouldMoveHeroAnimal)
            {
                // Eating Piece - Path Rule
                var movingPiece = MapPiece.HeroAnimal;
                var pathPiece = SelectNewPathPiece(knownMoves, p.MaxMoves, pieces);
                var from = heroLoc.Clone();
                var to = from.GetAdjacents().Where(x => x.IsInBounds(maxX, maxY) && !pieces.ContainsKey(x)).ToArray().Random();
                
                lb.MovePieceAndAddFloor(from, to, movingPiece);
                pieces[to] = movingPiece;
                lb.WithPieceAndFloor(from, pathPiece);
                pieces[from] = pathPiece;
                knownMoves++;
            }
            else
            {
                // Jumping Piece - Path Rule
                var movingPieceEntry = pieces.Where(x => x.Value.Rules().IsSelectable).Random();
                var movingPiece = movingPieceEntry.Value;
                var from = movingPieceEntry.Key;
                var toOptions = from.GetCardinals(2)
                    .Select(t => (t, t.InBetween(from)))
                    .Where(d => d.t.IsInBounds(maxX, maxY)
                                && !pieces.ContainsKey(d.t)
                                && d.Item2.Any(tweenTile => !pieces.ContainsKey(tweenTile)))
                    .ToArray();
                if (toOptions.Length == 0)
                {
                    Debug.LogWarning("Skipping a Cycle. Picked an impossible Selectable piece to move");
                    continue;
                }

                var option = toOptions.Random();
                var to = option.t;
                var tweens = option.Item2;
                
                lb.MovePieceAndAddFloor(from, to, movingPiece);
                pieces[to] = movingPiece;
                
                foreach (var tween in tweens)
                {
                    var newPiece = SelectNewPathPiece(knownMoves, p.MaxMoves, pieces);
                    lb.WithPieceAndFloor(tween, newPiece);
                    pieces[tween] = newPiece;
                }
                knownMoves++;
            }

            heroLoc = pieces.Single(x => x.Value == MapPiece.HeroAnimal).Key;
            isFinished = knownMoves >= p.MinMoves && knownMoves <= p.MaxMoves && pieces.Any(p => p.Value == MapPiece.StarFood);
            
            if (pieces.Count(x => x.Value == MapPiece.HeroAnimal) > 1)
                Debug.LogWarning("More than 1 Root Key");
            
            if (pieces.Count(x => x.Value == MapPiece.Barn) < 1)
                Debug.LogWarning("Less than 1 Root");
        }

        // Phase 3 - Finalization
        // Rule 3A - Ensure the Genius Path
        
        // Phase 4 - Map Optimization (Trim dead rows/columns)

        // TODO: Implement
        
        return lb.Build();
    }
    
    // TODO: Add some random floors?
    // Try in the App
    
}


