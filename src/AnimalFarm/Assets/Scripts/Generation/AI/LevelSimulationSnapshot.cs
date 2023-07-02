using System;
using System.Collections.Generic;
using System.Linq;

[Obsolete("New Version is in Level State Snapshot")]
public class LevelSimulationSnapshot
{
    private readonly int[,] _map;
    private readonly TilePoint _goal;
    private readonly bool _hasCollectedStar;
    public string Hash { get; }

    private LevelSimulationSnapshot(int[,] map, TilePoint goal, bool hasCollectedCube)
    {
        _map = map;
        _goal = goal;
        _hasCollectedStar = hasCollectedCube;
        Hash = _map.ToBytes().Md5Hash();
    }
    
    public LevelSimulationSnapshot(List<TilePoint> floors, Dictionary<TilePoint, MapPiece> pieces)
    {
        var map = new int[floors.Max(x => x.X) + 1, floors.Max(x => x.Y) + 1];
        floors.ForEach(x => map[x.X, x.Y] = (int)MapPiece.Floor);
        pieces.ForEach(x => map[x.Key.X, x.Key.Y] += (int)x.Value);
        _goal = pieces.Single(p => p.Value == MapPiece.Barn).Key;
        _hasCollectedStar = pieces.None(p => p.Value == MapPiece.StarFood);
        _map = map;
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
            }
        return moves;
    }

    public LevelSimulationSnapshot MakeMove(AIMove move)
    {
        var newMap = new int[_map.GetLength(0), _map.GetLength(1)];
        Buffer.BlockCopy(_map, 0, newMap, 0, _map.Length * sizeof(int));
        var piece = GetPiece(move.FromX, move.FromY);
        var hasCollectedCube = _hasCollectedStar;
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
        return new LevelSimulationSnapshot(newMap, _goal, hasCollectedCube);
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
        if (_hasCollectedStar)
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

    public bool IsGameOver() => HasRootKey(_goal.X, _goal.Y);
    public override int GetHashCode() => Hash.GetHashCode();
    public override bool Equals(object obj) => obj is LevelSimulationSnapshot item && Equals(item);
    public bool Equals(LevelSimulationSnapshot obj) => obj.Hash.Equals(Hash);

    private bool IsWithinBounds(int x, int y) => x >= 0 && y >= 0 && x < _map.GetLength(0) && y < _map.GetLength(1);
    private bool IsSelectable(int x, int y) => IsWithinBounds(x, y) && _map[x, y] > 16;
    private bool DoesJump(int x, int y) => IsWithinBounds(x, y) && IsSelectable(x, y) && _map[x, y] < 128;
    private bool IsJumpable(int x, int y) => IsWithinBounds(x, y) && _map[x, y] > 32;
    private bool IsWalkable(int x, int y) => IsWithinBounds(x, y) && _map[x, y] >= 1 && _map[x, y] < 8;
    private int GetPiece(int x, int y) => _map[x, y] % 2 == 1 ? _map[x, y] - 1 : _map[x, y] - 2;
    private bool IsFloor(int x, int y) => _map[x, y] % 2 == 1;
    private int IntBetween(int from, int to) => from + (to - from) / 2;
    private bool HasRootKey(int x, int y) => IsWithinBounds(x, y) && _map[x, y] > 16 && _map[x, y] < 19;
}
