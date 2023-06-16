using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSimulationSnapshot
{
    private readonly int[,] _map;
    private int _goalX;
    private int _goalY;
    private bool _hasCollectedCube;
    public string Hash { get; }

    private LevelSimulationSnapshot(int[,] map, int goalX, int goalY, bool hasCollectedCube)
    {
        _map = map;
        _goalX = goalX;
        _goalY = goalY;
        _hasCollectedCube = hasCollectedCube;
        Hash = _map.ToBytes().Md5Hash();
    }
    
    public LevelSimulationSnapshot(List<TilePoint> serverFloors, List<TilePoint> disengagedFailsafes,
        List<TilePoint> oneHealthSubroutines, List<TilePoint> twoHealthSubroutines, List<TilePoint> iceSubroutines, List<TilePoint> dataCubes,
        TilePoint rootKey, TilePoint root)
    {
        _map = new int[serverFloors.Concat(disengagedFailsafes).Concat(root).Max(x => x.X) + 1, serverFloors.Concat(disengagedFailsafes).Concat(root).Max(x => x.Y) + 1];
        serverFloors.ForEach(x => _map[x.X, x.Y] = (int)GamePosition.Floor);
        disengagedFailsafes.ForEach(x => _map[x.X, x.Y] = (int)GamePosition.DisengagedFailsafe);
        oneHealthSubroutines.ForEach(x => _map[x.X, x.Y] += (int)GamePosition.Subroutine);
        twoHealthSubroutines.ForEach(x => _map[x.X, x.Y] += (int)GamePosition.DoubleSubroutine);
        iceSubroutines.ForEach(x => _map[x.X, x.Y] += (int)GamePosition.IceSubroutine);
        dataCubes.ForEach(x => _map[x.X, x.Y] += (int)GamePosition.DataCube);
        _map[rootKey.X, rootKey.Y] += (int)GamePosition.RootKey;
        _map[root.X, root.Y] += (int)GamePosition.Root;
        _goalX = root.X;
        _goalY = root.Y;
        _hasCollectedCube = !dataCubes.Any();
        Hash = _map.ToBytes().Md5Hash();
    }
    
    public List<AIMove> GetMoves()
    {
        var moves = new List<AIMove>();
        for (var x = 0; x < _map.GetLength(0); x++)
            for (var y = 0; y < _map.GetLength(1); y++)
            {
                if (DoesJump(x, y))
                {
                    if (IsJumpable(x + 1, y) && IsWalkable(x + 2, y))
                        moves.Add(new AIMove(x, y, x + 2, y));
                    if (IsJumpable(x - 1, y) && IsWalkable(x - 2, y))
                        moves.Add(new AIMove(x, y, x - 2, y));
                    if (IsJumpable(x, y + 1) && IsWalkable(x, y + 2))
                        moves.Add(new AIMove(x, y, x, y + 2));
                    if (IsJumpable(x, y - 1) && IsWalkable(x, y - 2))
                        moves.Add(new AIMove(x, y, x, y - 2));
                }
                else if (DoesTeleport(x, y))
                {
                    if (IsWalkable(x + 3, y))
                        moves.Add(new AIMove(x, y, x + 3, y));
                    if (IsWalkable(x - 3, y))
                        moves.Add(new AIMove(x, y, x - 3, y));
                    if (IsWalkable(x, y + 3))
                        moves.Add(new AIMove(x, y, x, y + 3));
                    if (IsWalkable(x, y - 3))
                        moves.Add(new AIMove(x, y, x, y - 3));
                }
            }
        return moves;
    }

    public LevelSimulationSnapshot MakeMove(AIMove move)
    {
        var newMap = new int[_map.GetLength(0), _map.GetLength(1)];
        Buffer.BlockCopy(_map, 0, newMap, 0, _map.Length * sizeof(int));
        var piece = GetPiece(move.FromX, move.FromY);
        var hasCollectedCube = _hasCollectedCube;
        if (newMap[move.ToX, move.ToY] > 4)
        {
            newMap[move.ToX, move.ToY] -= 4;
            hasCollectedCube = true;
        }
        newMap[move.ToX, move.ToY] += piece;
        newMap[move.FromX, move.FromY] = IsFloor(move.FromX, move.FromY) ? 1 : 0;
        if (piece < 128)
        {
            var damagedPiece = GetPiece(IntBetween(move.FromX, move.ToX), IntBetween(move.FromY, move.ToY));
            if (damagedPiece == 64)
                newMap[IntBetween(move.FromX, move.ToX), IntBetween(move.FromY, move.ToY)] -= 32;
            else
                newMap[IntBetween(move.FromX, move.ToX), IntBetween(move.FromY, move.ToY)] -= damagedPiece;
        }
        return new LevelSimulationSnapshot(newMap, _goalX, _goalY, hasCollectedCube);
    }

    public bool HasWon()
    {
        if (!IsGameOver())
            return false;
        var count = 0;
        foreach (var space in _map)
            if (space > 8)
                count++;
        return count == 1;
    }

    public int GetStars()
    {
        var stars = 0;
        if (IsGameOver())
            stars++;
        if (_hasCollectedCube)
            stars++;
        stars++;
        foreach (var space in _map)
        {
            if (space > 32)
            {
                stars--;
                break;
            }
        }
        return stars;
    }

    public bool IsGameOver() => HasRootKey(_goalX + 1, _goalY) || HasRootKey(_goalX - 1, _goalY) || HasRootKey(_goalX, _goalY + 1) || HasRootKey(_goalX, _goalY - 1);

    public override int GetHashCode() => Hash.GetHashCode();
    public override bool Equals(object obj) => obj is LevelSimulationSnapshot item && Equals(item);
    public bool Equals(LevelSimulationSnapshot obj) => obj.Hash.Equals(Hash);

    private bool IsWithinBounds(int x, int y) => x >= 0 && y >= 0 && x < _map.GetLength(0) && y < _map.GetLength(1);
    private bool IsSelectable(int x, int y) => IsWithinBounds(x, y) && _map[x, y] > 16;
    private bool DoesJump(int x, int y) => IsWithinBounds(x, y) && IsSelectable(x, y) && _map[x, y] < 128;
    private bool DoesTeleport(int x, int y) => IsWithinBounds(x, y) && _map[x, y] > 128;
    private bool IsJumpable(int x, int y) => IsWithinBounds(x, y) && _map[x, y] > 32;
    private bool IsWalkable(int x, int y) => IsWithinBounds(x, y) && _map[x, y] >= 1 && _map[x, y] < 8;
    private int GetPiece(int x, int y) => _map[x, y] % 2 == 1 ? _map[x, y] - 1 : _map[x, y] - 2;
    private bool IsFloor(int x, int y) => _map[x, y] % 2 == 1;
    private int IntBetween(int from, int to) => from + (to - from) / 2;
    private bool HasRootKey(int x, int y) => IsWithinBounds(x, y) && _map[x, y] > 16 && _map[x, y] < 19;

    [Flags]
    private enum GamePosition
    {
        Nothing = 0,
        Floor = 1,
        DisengagedFailsafe = 2,
        DataCube = 4,
        Root = 8,
        RootKey = 16,
        Subroutine = 32,
        DoubleSubroutine = 64,
        IceSubroutine = 128,
    }
}
