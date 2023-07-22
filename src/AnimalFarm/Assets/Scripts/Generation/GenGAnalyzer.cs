using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GMoveBranchStats
{
    public PossibleGOutcomes Outcome = PossibleGOutcomes.Unfinished;
    public TilePoint[] Path = Array.Empty<TilePoint>();
    public int NumMoves => Path.Length;

    public override string ToString() => $"{Outcome} - {NumMoves} - [{string.Join(",", Path.Select(p => p.ToString()))}]";
}

public enum PossibleGOutcomes
{
    Unfinished = 0,
    DeadEnd = 1,
    GPathComplete = 2,
}

public class GenGAnalyzer
{
    private HashSet<TilePoint> _pastLocs = new HashSet<TilePoint>();
    private Dictionary<LevelStateSnapshot, GMoveBranchStats> _outcomes = new Dictionary<LevelStateSnapshot, GMoveBranchStats>();

    private const string analysisEngineVersion = "0.1";

    public static GenGAnalyzer Analyze(LevelMap map, bool forCreation = false)
    {
        var a = new GenGAnalyzer();
        a.Analyze(map.ToState(), forCreation);
        return a;
    }

    public Dictionary<LevelStateSnapshot, GMoveBranchStats> Outcomes => _outcomes;

    private void Analyze(LevelStateSnapshot state, bool forCreation = false)
    {
        _outcomes = new Dictionary<LevelStateSnapshot, GMoveBranchStats>();

        // 1. Analyze Full Move Tree
        RecursiveCalculateMoveTree(state, Array.Empty<TilePoint>(), forCreation);
    }

    private GMoveBranchStats RecursiveCalculateMoveTree(LevelStateSnapshot state, TilePoint[] path, bool forCreation = false)
    {
        var heroLoc = state.Pieces.Single(p => p.Value == MapPiece.HeroAnimal).Key;
        var moves = state.GetGPartialMoves(heroLoc, allowNoFloor: forCreation).Where(m => !_pastLocs.Contains(m)).ToArray();
        var outcome = moves.Any() ? PossibleGOutcomes.Unfinished : PossibleGOutcomes.DeadEnd;
        _outcomes[state] = new GMoveBranchStats { Outcome = outcome, Path = path };
        Log.SInfo(LogScopes.Analysis, $"Depth: {path.Length}, HeroLoc: {heroLoc} - {_outcomes[state]}. Possible Moves: [{string.Join("->", moves.Select(x => x.ToString()))}]");
        foreach (var move in moves)
        {
            Log.SInfo(LogScopes.Analysis, "Move: " + move);
            _pastLocs.Add(move);
            var newState = state.GPartialMoveHeroAnimal(move);
            if (_outcomes.ContainsKey(newState))
            {
                Log.SInfo(LogScopes.Analysis, $"Duplicate State");
            }
            else if (newState.HasWon())
            {
                _outcomes[newState] = new GMoveBranchStats { Outcome = PossibleGOutcomes.GPathComplete, Path = path.Concat(move).ToArray() };
                Log.SInfo(LogScopes.Analysis, "Won: " +  _outcomes[newState]);
            }
            else
            {
                RecursiveCalculateMoveTree(newState, path = path.Concat(move).ToArray(), forCreation);
            }
        }

        return _outcomes[state];
    }
}
