using System;
using System.Collections.Generic;

public class GenAnalyzer
{
    private LevelStateSnapshot _initialState;
    private Dictionary<LevelStateSnapshot, MoveBranchStats> _outcomes;
    private int _numberOfDeadBranches;
    private int _numberOfWinningBranches;

    private const float analysisEngineVersion = 0.1f;

    public static GenAnalysisResult Analyze(LevelMap map)
        => new GenAnalyzer().Analyze(map.ToState());
    
    private GenAnalysisResult Analyze(LevelStateSnapshot state)
    {
        _initialState = state;
        _outcomes = new Dictionary<LevelStateSnapshot, MoveBranchStats>();
        _numberOfDeadBranches = 0;
        _numberOfWinningBranches = 0;
        
        // 1. Analyze Full Move Tree
        RecursiveCalculateMoveTree(state, 0);

        var data = new GenAnalysisResult
        {
            AnalysisEngineVersion = analysisEngineVersion,
            
            GameLosingMistakeCount = _numberOfDeadBranches,
            MechanicComplexity = 1, // TODO: Enhance this as more mechanics are added
            // TODO: Implement Unneeded Tile Counting
        };
        foreach (var outcome in _outcomes.Values)
        {
            var o = outcome.Outcomes;
            var numMoves = outcome.NumMoves;
            if (data.StandardWinPossible && o >= PossibleOutcomes.ThreeOrangeStar)
                data.StandardWinPossible = true;

            data.MaxPossibleMoves = Math.Max(data.MaxPossibleMoves, numMoves);
            var isWin = o >= PossibleOutcomes.DeadEnd;
            if (isWin)
            {
                data.MinMovesNeededToSolve = Math.Min(data.MinMovesNeededToSolve, numMoves);
                data.MaxMovesNeededToSolve = Math.Max(data.MaxMovesNeededToSolve, numMoves);
            }
        }

        data.ComplexityScore = (data.MaxPossibleMoves + 2 * data.UnneededTilesCount) * data.MechanicComplexity;
        data.ComplexityDifficulty = data.ComplexityScore * (_numberOfDeadBranches / _outcomes.Count); 
        
        return data;
    }

    [Serializable]
    private class MoveBranchStats
    {
        public PossibleOutcomes Outcomes;
        public int NumMoves;
    }

    private MoveBranchStats RecursiveCalculateMoveTree(LevelStateSnapshot state, int numPreviousMoves)
    {
        var moveNumber = numPreviousMoves + 1;

        _outcomes[state] = new MoveBranchStats { Outcomes = PossibleOutcomes.Uncalculated, NumMoves = moveNumber };
        foreach (var move in state.GetPossibleMoves().ToArray())
        {
            var newState = state.ApplyMove(move);
            if (_outcomes.ContainsKey(newState)) { }
            else if (newState.HasWon())
            {
                _numberOfWinningBranches++;
                var numOrangeStarsCollected = 1;
                if (state.Pieces.Values.None(p => p == MapPiece.StarFood))
                    numOrangeStarsCollected++;
                if (state.Counters.ValueOrDefault(CounterType.NumStarFoodCollected, () => 0) > 0)
                    numOrangeStarsCollected++;
                
                if (numOrangeStarsCollected == 3)
                    _outcomes[state].Outcomes |= PossibleOutcomes.ThreeOrangeStar;
                else if (numOrangeStarsCollected == 2)
                    _outcomes[state].Outcomes |= PossibleOutcomes.TwoOrangeStar;
                else
                    _outcomes[state].Outcomes |= PossibleOutcomes.OneOrangeStar;
            }
            else
            {
                var subbranch = RecursiveCalculateMoveTree(newState, moveNumber);
                _outcomes[state].Outcomes |= subbranch.Outcomes;
                _outcomes[state].NumMoves = subbranch.NumMoves;
            }
        }
        if ((int)_outcomes[state].Outcomes == 0)
            _numberOfDeadBranches++;
        if ((int)_outcomes[state].Outcomes <= 1)
            _outcomes[state].Outcomes |= PossibleOutcomes.DeadEnd;
        return _outcomes[state];
    }

    [Flags]
    private enum PossibleOutcomes
    {
        Uncalculated = 0,
        DeadEnd = 1,
        OneOrangeStar = 2,
        TwoOrangeStar = 4,
        ThreeOrangeStar = 8
    }
}
