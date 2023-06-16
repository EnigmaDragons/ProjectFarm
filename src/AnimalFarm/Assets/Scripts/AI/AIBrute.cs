using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

public class AIBrute
{
    private Dictionary<string, LevelSimulationSnapshot> _oldStates;
    private List<AIMove> _movesToWin;
    private int _numCalculationSteps;
    
    public bool CanWin { get; private set; }
    public AIMove NextMove => _movesToWin.Last();
    
    public bool CalculateSolution(LevelSimulationSnapshot state)
    {
        var sw = Stopwatch.StartNew();
        _oldStates = new Dictionary<string, LevelSimulationSnapshot>();
        _movesToWin = new List<AIMove>();
        _numCalculationSteps = 0;
        CanWin = RecursiveCalculateSolution(state);
        Debug.Log($"AI Brute: Iterations {_numCalculationSteps++} in {sw.ElapsedMilliseconds}ms");
        return true;
    }

    private bool RecursiveCalculateSolution(LevelSimulationSnapshot state)
    {
        _numCalculationSteps++;
        _oldStates[state.Hash] = state;
        foreach (var move in state.GetMoves().ToArray().Shuffled())
        {
            var newState = state.MakeMove(move);
            if (_oldStates.ContainsKey(newState.Hash)) continue; // Already examined this tree
            
            if (newState.IsGameOver())
            {
                if (newState.HasWon())
                {
                    _movesToWin.Add(move);
                    return true;
                }
            }
            else if (RecursiveCalculateSolution(newState))
            {
                _movesToWin.Add(move);
                return true;
            }
        }
        return false;
    }
}
