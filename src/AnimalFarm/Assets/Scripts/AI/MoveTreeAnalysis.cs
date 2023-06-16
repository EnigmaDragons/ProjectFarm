using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MoveTreeAnalysis
{
    private string _initialState;
    private Dictionary<string, LevelSimulationSnapshot> _oldStates;
    private Dictionary<string, PossibleOutcomes> _possibleOutcomes;
    private int _numberOfDeadBranches;
    private int _numberOfWinningBranches;

    public MoveTreeData CalculateMoveTree(LevelSimulationSnapshot state)
    {
        _initialState = state.Hash;
        _oldStates = new Dictionary<string, LevelSimulationSnapshot>();
        _possibleOutcomes = new Dictionary<string, PossibleOutcomes>();
        _numberOfDeadBranches = 0;
        _numberOfWinningBranches = 0;
        RecursiveCalculateMoveTree(state, state.Hash);
        var data = new MoveTreeData
        {
            HasThreeStar = (_possibleOutcomes[_initialState] & PossibleOutcomes.ThreeStar) != 0,
            HasTwoStar = (_possibleOutcomes[_initialState] & PossibleOutcomes.TwoStar) != 0,
            HasOneStar = (_possibleOutcomes[_initialState] & PossibleOutcomes.OneStar) != 0,
            NumberOfDeadBranches = _numberOfDeadBranches,
            NumberOfWinningBranches = _numberOfWinningBranches
        };
        new JsonFileStored<MoveTreeData>(Path.Combine(Application.persistentDataPath, "MoveTree.json"), () => new MoveTreeData()).Write(_ => data);
        return data;
    }

    private PossibleOutcomes RecursiveCalculateMoveTree(LevelSimulationSnapshot state, string stateHash)
    {
        _oldStates[stateHash] = state;
        _possibleOutcomes[stateHash] = PossibleOutcomes.Uncalculated;
        foreach (var move in state.GetMoves().ToArray().Shuffled())
        {
            var newState = state.MakeMove(move);
            var newStateHash = newState.Hash;
            if (_oldStates.ContainsKey(newStateHash))
            {
                _possibleOutcomes[stateHash] = _possibleOutcomes[stateHash] | _possibleOutcomes[newStateHash];
            }
            else if (newState.IsGameOver())
            {
                _numberOfWinningBranches++;
                var stars = newState.GetStars();
                if (stars == 3)
                    _possibleOutcomes[stateHash] = _possibleOutcomes[stateHash] | PossibleOutcomes.ThreeStar;
                else if (stars == 2)
                    _possibleOutcomes[stateHash] = _possibleOutcomes[stateHash] | PossibleOutcomes.TwoStar;
                else
                    _possibleOutcomes[stateHash] = _possibleOutcomes[stateHash] | PossibleOutcomes.OneStar;
            }
            else
                _possibleOutcomes[stateHash] = _possibleOutcomes[stateHash] | RecursiveCalculateMoveTree(newState, newStateHash);
        }
        if ((int)_possibleOutcomes[stateHash] == 0)
            _numberOfDeadBranches++;
        if ((int) _possibleOutcomes[stateHash] <= 1)
            _possibleOutcomes[stateHash] = _possibleOutcomes[stateHash] | PossibleOutcomes.DeadEnd;
        return _possibleOutcomes[stateHash];
    }

    [Flags]
    private enum PossibleOutcomes
    {
        Uncalculated = 0,
        DeadEnd = 1,
        OneStar = 2,
        TwoStar = 4,
        ThreeStar = 8
    }

    public class MoveTreeData
    {
        public bool HasThreeStar;
        public bool HasTwoStar;
        public bool HasOneStar;
        public int NumberOfDeadBranches;
        public int NumberOfWinningBranches;
    }
}
