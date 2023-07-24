using System;
using System.Collections.Generic;
using System.Linq;

public class GenContextData
{
    public int KnownMoves;
    public int MaxMoves;
    public Dictionary<TilePoint, MapPiece> Pieces;
    public HashSet<MapPiece> MustInclude = new HashSet<MapPiece>();

    public int MaxRemainingMoves => MaxMoves - KnownMoves;
}

public static class LevelGenV1
{
    private static MapPieceGenRule SelectNewPathPiece(GenContextData ctx)
    {
        var rules = new MapPieceGenRule[]
        {
            new TreatPlacementRule(),
            new DinoFissurePlacementRule(),
            new DolphinRidePlacementRule(),
            new ElephantPlacementRule(),
            new FoodPlacementRule()
        }.OrderBy(r => r.Priority).ToArray();

        foreach (var rule in rules)
            if (rule.MustPlace(ctx))
            {
                Log.SInfo(LogScopes.Gen, $"Must Place: {rule.Piece}");
                return rule;
            }

        foreach (var rule in rules)
            if (rule.ShouldPlace(ctx))
                return rule;

        return new FoodPlacementRule();
    }

    private static Maybe<LevelMap> GenerateInner(LevelGenV1Params p)
    {
        var mustIncludes = new HashSet<MapPiece>(p.MustInclude);
        var maxX = 10;
        var maxY = 7;
        var lb = new LevelMapBuilder(Guid.NewGuid().ToString(), maxX, maxY, new HashSet<MapPiece>(new [] { MapPiece.Fissure }));

        var pieces = new Dictionary<TilePoint, MapPiece>();
        var specialFloors = new Dictionary<TilePoint, MapPiece>();
        var includes = new HashSet<MapPiece>();
        
        // Phase 1 - Initial Setup
        // Rule 1A - Add a Barn
        var barnLoc = new TilePoint(Rng.Int(0, maxX), Rng.Int(0, maxY));
        lb.WithPieceAndFloor(barnLoc, MapPiece.Barn, MapPiece.Dirt);
        pieces[barnLoc] = MapPiece.Barn;
        Log.SInfo(LogScopes.Gen, $"Placed {MapPiece.Barn} at {barnLoc}");
        
        // Rule 1B - Place a Hero Animal
        var adjacents = barnLoc.GetAdjacents().Where(x => x.IsInBounds(maxX, maxY)).ToArray();
        var heroLoc = adjacents.Random();
        lb.WithPieceAndFloor(heroLoc, MapPiece.HeroAnimal, MapPiece.Dirt);
        pieces[heroLoc] = MapPiece.HeroAnimal;
        heroLoc = pieces.Single(x => x.Value == MapPiece.HeroAnimal).Key;
        Log.SInfo(LogScopes.Gen, $"Placed {MapPiece.HeroAnimal} at {heroLoc}");
        
        // Phase 2 - Puzzle Meat
        var knownMoves = 0;
        var isFinished = false;
        var genSteps = 0;

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
                    SpecialFloors = specialFloors,
                    Includes = includes,
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
                Log.Warn(message);
                return Maybe<LevelMap>.Missing();
            }
            else
            {
                Log.Warn($"Should not be hitting this branch, unless another Piece Type is Selectable. Selectable Pieces are {string.Join(", ", nonHeroSelectablePieces.Select(x => x.Value))}");
            }

            heroLoc = pieces.Single(x => x.Value == MapPiece.HeroAnimal).Key;
            isFinished = knownMoves >= p.MaxMoves ||
                         (knownMoves >= p.MinMoves
                         && heroLoc.DistanceFrom(barnLoc) != 1
                         && pieces.Any(piece => piece.Value == MapPiece.Treat)
                         && mustIncludes.All(i => pieces.Any(piece => piece.Value == i))
                         && Rng.Dbl() < 1 - p.ContinuationOdds);

            if (pieces.Count(x => x.Value == MapPiece.HeroAnimal) > 1)
                Log.Warn("More than 1 Root Key");
            
            if (pieces.Count(x => x.Value == MapPiece.Barn) < 1)
                Log.Warn("Less than 1 Root");
            
            genSteps++;
            if (genSteps > 1000)
                throw new Exception("Gen Steps exceeded 1000");
        }

        // Phase 3 - Finalization
        // Rule 3A - Ensure the Genius Path
        // TODO: Add some random floors?
        if (!p.SkipG)
        {
            Log.SInfo(LogScopes.Gen, $"Genius - Hero: {heroLoc}. Barn: {barnLoc}");
            NaiveGAlgo(lb);
        }
 
        if (p.SkipOptimization)
            return lb.Build();
        
        // Phase 4 - Map Optimization 
        // Rule 4A - Trim
        // Rule 4B - Flip X/Y if Taller than Wide
        var trimmed = lb.BuildTrimmed();
        var flippedIfNeeded = FlipXyIfTallerThanWide(trimmed);
        return flippedIfNeeded;
    }

    private static void AStarGAlgo(LevelMapBuilder lb)
    {
        var path = GeniusPathAStar.GetBestPath(lb.Build(), allowMovementOnNothingFloor: true);
        if (path.Length > 0)
        {
            var branch = path;
            Log.SInfo(LogScopes.Gen, $"Genius - Selected Path: {string.Join("->", branch.Select(b => b.ToString()))}");
            lb.WithHero((HeroAnimal)(branch.Length - 1));
            foreach (var tile in branch)
                lb.WithFloorIfMissing(tile, MapPiece.Dirt);
        }
        else
        {
            Log.Warn("No Genius Path Possible");
            throw new Exception("No Genius Path Possible");
        }
    }

    private static void NaiveGAlgo(LevelMapBuilder lb)
    {
        var tree = GenGAnalyzer.Analyze(lb.Build(), 11, forCreation: true);
        var possiblePaths = tree.Outcomes.Where(x => x.Outcome == PossibleGOutcomes.GPathComplete).ToArray();
        var pathsInRange = possiblePaths.Where(x => x.NumMoves >= 2 && x.NumMoves <= 11).ToArray();
        Log.SInfo(LogScopes.Gen, $"Genius - Animal Options: {string.Join(Environment.NewLine, pathsInRange.Select(x => x.ToString()))}. Total Options: {possiblePaths.Length}");
        if (pathsInRange.Any())
        {
            var branch = pathsInRange.OrderBy(x => x.NumMoves).First();
            Log.SInfo(LogScopes.Gen, $"Genius - Selected Path: {branch.NumMoves} {string.Join("->", branch.Path.Select(b => b.ToString()))}");
            lb.WithHero((HeroAnimal)(branch.NumMoves - 1));
            foreach (var tile in branch.Path)
                lb.WithFloorIfMissing(tile, MapPiece.Dirt);
        }
        else
        {
            Log.Warn("No Genius Path Possible");
            throw new Exception("No Genius Path Possible");
        }
    }

    private static LevelMap FlipXyIfTallerThanWide(LevelMap level)
    {
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
                var maybeLevel = GenerateInner(p);
                if (maybeLevel.IsPresent)
                    return maybeLevel.Value;
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.SInfo(LogScopes.Gen, string.Format("Failed to Generate Map. Retrying... Attempt {0} of {1}", attemptNumber, maxAttempts));
            }
        }
        
        throw new Exception($"Failed to Generate Map after {maxAttempts} attempts.");
    }
}
