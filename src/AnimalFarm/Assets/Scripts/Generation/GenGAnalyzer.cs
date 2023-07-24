using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GMoveBranchStats
{
    public PossibleGOutcomes Outcome = PossibleGOutcomes.Unfinished;
    public TilePoint[] Path = Array.Empty<TilePoint>();
    public int NumMoves => Path.Length - 1;

    public override string ToString() => $"{Outcome} - {NumMoves} - [{string.Join(" -> ", Path.Select(p => p.ToString()))}]";
}

public enum PossibleGOutcomes
{
    Unfinished = 0,
    DeadEnd = 1,
    GPathComplete = 2,
}

public class GenGAnalyzer
{
    private Dictionary<string, GMoveBranchStats> _outcomes = new Dictionary<string, GMoveBranchStats>();

    private const string analysisEngineVersion = "0.1";

    public static GenGAnalyzer Analyze(LevelMap map, int maxMoves, bool forCreation = false)
    {
        var a = new GenGAnalyzer();
        a.Analyze(map.ToState(), maxMoves, forCreation);
        return a;
    }

    public GMoveBranchStats[] Outcomes => _outcomes.Values.ToArray();

    private void Analyze(LevelStateSnapshot state, int maxMoves, bool forCreation = false)
    {
        _outcomes = new Dictionary<string, GMoveBranchStats>();

        // 1. Analyze Full Move Tree
        var heroLoc = state.Pieces.Single(p => p.Value == MapPiece.HeroAnimal).Key;
        RecursiveCalculateMoveTree(state, new [] { heroLoc }, maxMoves, forCreation);
    }

    private static string ToPathString(TilePoint[] path) => string.Join("|", path.Select(p => p.ToString()));
    
    private GMoveBranchStats RecursiveCalculateMoveTree(LevelStateSnapshot state, TilePoint[] path, int maxMoves, bool forCreation = false)
    {
        var depth = path.Length - 1;
        var heroLoc = state.Pieces.Single(p => p.Value == MapPiece.HeroAnimal).Key;
        var possibleMoves = state.GetGPartialMoves(heroLoc, allowNoFloor: forCreation).Where(m => !path.Contains(m)).ToArray();
        var outcome = possibleMoves.Any() ? PossibleGOutcomes.Unfinished : PossibleGOutcomes.DeadEnd;
        var pathString = ToPathString(path);
        _outcomes[pathString] = new GMoveBranchStats { Outcome = outcome, Path = path };
        Log.SInfo(LogScopes.Analysis, $"Depth: {depth}, HeroLoc: {heroLoc} - {_outcomes[pathString]}. Possible Moves: [{string.Join("|", possibleMoves.Select(x => x.ToString()))}]");
        foreach (var move in possibleMoves)
        {
            var newPath = path.Concat(move).ToArray();
            var newPathString = ToPathString(newPath);
            var newState = state.GPartialMoveHeroAnimal(move);
            if (_outcomes.ContainsKey(newPathString))
            {
                Log.SInfo(LogScopes.Analysis, $"Move: {move} - Duplicate State");
            }
            else if (newState.HasWon())
            {
                _outcomes[newPathString] = new GMoveBranchStats { Outcome = PossibleGOutcomes.GPathComplete, Path = newPath };
                Log.SInfo(LogScopes.Analysis, "Move: {move} - Won: " +  _outcomes[newPathString]);
            }
            else if (depth < maxMoves)
            {
                RecursiveCalculateMoveTree(newState, newPath, maxMoves, forCreation);
            }
            else
            {
                Log.SInfo(LogScopes.Analysis, $"Move: {move} - Max Moves Reached without a Finish");
            }
        }

        return _outcomes[pathString];
    }
}
