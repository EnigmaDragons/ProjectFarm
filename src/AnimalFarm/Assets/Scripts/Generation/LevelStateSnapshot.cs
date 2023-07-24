using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelStateSnapshot
{
    public Vector2Int Size { get; }
    public Dictionary<TilePoint, MapPiece> Floors { get; }
    public Dictionary<TilePoint, MapPiece> Pieces { get; }
    public Dictionary<CounterType, int> Counters { get; }
    public string Hash { get; }

    public LevelStateSnapshot(Vector2Int size, Dictionary<TilePoint, MapPiece> floors, Dictionary<TilePoint, MapPiece> pieces,
        Dictionary<CounterType, int> counters)
    {
        Size = size;
        Floors = floors;
        Pieces = pieces;
        Counters = counters;
        Hash = LevelStateSnapshotExtensions.GetHash(floors, pieces, counters);
    }
    
    public override int GetHashCode() => Hash.GetHashCode();
    public override bool Equals(object obj) => obj is LevelStateSnapshot item && Equals(item);
    public bool Equals(LevelStateSnapshot obj) => obj.Hash.Equals(Hash);

    public bool HasWon() => Pieces.Values.None(p => p == MapPiece.HeroAnimal);
}

public class LevelPlayPossibleMove
{
    public MovementType MovementType;
    public MapPiece Piece;
    public TilePoint From;
    public TilePoint To;
}

public static class LevelStateSnapshotExtensions
{
    public static LevelStateSnapshot ToState(this LevelMap lm)
    {
        var floorDict = new Dictionary<TilePoint, MapPiece>();
        var pieceDict = new Dictionary<TilePoint, MapPiece>();
        lm.GetIterator().ForEach(t =>
        {
            var (x, y) = t;
            var point = new TilePoint(x, y);
            if (lm.FloorLayer[x, y] != MapPiece.Nothing)
                floorDict[point] = lm.FloorLayer[x, y];
            if (lm.ObjectLayer[x, y] != MapPiece.Nothing)
                pieceDict[point] = lm.ObjectLayer[x, y];
        });
        return new LevelStateSnapshot(lm.Size, floorDict, pieceDict, new DictionaryWithDefault<CounterType, int>(0));
    }
    
    public static string GetHash(Dictionary<TilePoint, MapPiece> floors, Dictionary<TilePoint, MapPiece> pieces,
        Dictionary<CounterType, int> counters)
    {
        var maxX = Math.Max(floors.Keys.Max(x => x.X), pieces.Keys.Max(x => x.X));
        var maxY = Math.Max(floors.Keys.Max(x => x.Y), pieces.Keys.Max(x => x.Y));
        var map = new int[maxX + 1, maxY + 1];
        floors.ForEach(x => map[x.Key.X, x.Key.Y] += (int)x.Value);
        pieces.ForEach(x => map[x.Key.X, x.Key.Y] += (int)x.Value);
        return map.ToBytes().Md5Hash() + string.Join("|", counters.Select(v => $"{v.Key}:{v.Value}"));
    }
    
    public static List<TilePoint> GetGPartialMoves(this LevelStateSnapshot state, TilePoint origin, bool allowNoFloor = false)
    {
        var adjacents = origin.GetAdjacents();
        bool IsInBounds(TilePoint p) => p.IsInBounds(state.Size);
        bool IsWalkable(TilePoint p) => state.Floors.ContainsKey(p) && state.Floors[p].Rules().IsWalkable && !state.Pieces.ContainsKey(p);
        bool IsUnplacedFloor(TilePoint p) => allowNoFloor && (!state.Floors.ContainsKey(p) || state.Floors[p] == MapPiece.Nothing);
        bool IsBarn(TilePoint p) => state.Pieces.ContainsKey(p) && state.Pieces[p] == MapPiece.Barn;
        var shouldLogEval = true;
        
        return adjacents
            .Where(a =>
            {
                if (shouldLogEval)
                {
                    var isInBounds = IsInBounds(a);
                    var isBarn = IsBarn(a);
                    var isUnplacedFloor = IsUnplacedFloor(a);
                    var isWalkable = IsWalkable(a);
                    var isValid = isInBounds && (isBarn || isUnplacedFloor || isWalkable);
                    if (!isValid)
                    {
                        if (!isInBounds)
                            Log.SInfo(LogScopes.Analysis, $"{a} - Out of Bounds");
                        else
                        {
                            if (!isBarn)
                                Log.SInfo(LogScopes.Analysis, $"{a} - Not Barn");
                            if (!allowNoFloor)
                                Log.SInfo(LogScopes.Analysis, $"Unplaced Floors Not Allowed");
                            if (!isUnplacedFloor)
                                Log.SInfo(LogScopes.Analysis, $"{a} - Not Unplaced Floor");
                            if (!isWalkable)
                                Log.SInfo(LogScopes.Analysis, $"{a} - Not Walkable");
                        }
                    }
                }

                return IsInBounds(a) && (IsBarn(a) || IsUnplacedFloor(a) || IsWalkable(a));
            }).ToList();
    }
    
    public static List<LevelPlayPossibleMove> GetPossibleMoves(this LevelStateSnapshot state, TilePoint forPiece = null)
    {
        var possibleMoves = new List<LevelPlayPossibleMove>();
        var pieceMoveTypes = state.Pieces
            .Where(x =>x.Value.IsSelectable() && (forPiece == null || x.Key.Equals(forPiece)))
            .SelectMany(s => s.Value.Rules().MovementTypes.Select(mt => (s, mt))).ToArray();
        foreach (var pieceMoveType in pieceMoveTypes)
        {
            Log.SInfo(LogScopes.Movement, $"Piece: {pieceMoveType.s.Value} - {pieceMoveType.mt}");
            var piece = pieceMoveType.s.Value;
            var moveType = pieceMoveType.mt;
            var pieceTile = pieceMoveType.s.Key;
            if (moveType == MovementType.Eat || moveType == MovementType.Enter || moveType == MovementType.SwimRide || moveType == MovementType.Activate)
                foreach (var adjTile in pieceTile.GetAdjacents())
                {
                    var can = state.Can(moveType, pieceTile, adjTile);
                    // NOTE: For Activation Debugging
                    // if (!can && moveType == MovementType.Activate) 
                    //     Log.SInfo(LogScopes.Hints, $"Can't Activate - {pieceTile} -> {adjTile} - {can}");
                    if (can)
                        possibleMoves.Add(new LevelPlayPossibleMove
                            { Piece = piece, MovementType = moveType, From = pieceTile, To = adjTile });
                }

            if (pieceMoveType.mt == MovementType.Jump)
                foreach (var jumpTargetTile in pieceTile.GetCardinals(2))
                {
                    var canJump = state.Can(moveType, pieceTile, jumpTargetTile);
                    Log.SInfo("Move Logic", $"Can Jump - {pieceTile} -> {jumpTargetTile} - {canJump}");
                    if (canJump)
                        possibleMoves.Add(new LevelPlayPossibleMove
                            { Piece = piece, MovementType = moveType, From = pieceTile, To = jumpTargetTile });
                }
        }
        return possibleMoves;
    }
    
    public static LevelStateSnapshot ApplyMove(this LevelStateSnapshot s, LevelPlayPossibleMove move)
    {
        if (move.MovementType == MovementType.Jump)
        {
            var pieces = s.Pieces.ToDictionary(k => k.Key, v => v.Value);
            var inBetweenTiles = move.To.InBetween(move.From);
            var piecesJumped = new List<MapPiece>();
            foreach (var jumpedTile in inBetweenTiles)
            {
                if (pieces.TryGetValue(jumpedTile, out var jumpedPiece))
                {
                    piecesJumped.Add(jumpedPiece);
                    pieces.Remove(jumpedTile);
                }
            }

            pieces.Remove(move.From);
            pieces[move.To] = move.Piece;

            var counters = s.Counters.WithUpdatedCounters(piecesJumped);
            return new LevelStateSnapshot(s.Size, s.Floors, pieces, counters);
        }

        if (move.MovementType == MovementType.Eat)
        {
            var piecesEaten = new List<MapPiece>();
            var pieces = s.Pieces.ToDictionary(k => k.Key, v => v.Value);
            if (pieces.TryGetValue(move.To, out var jumpedPiece))
            {
                piecesEaten.Add(jumpedPiece);
                pieces.Remove(move.To);
            }
            
            pieces.Remove(move.From);
            pieces[move.To] = move.Piece;
            
            var counters = s.Counters.WithUpdatedCounters(piecesEaten);
            return new LevelStateSnapshot(s.Size, s.Floors, pieces, counters);
        }
        
        if (move.MovementType == MovementType.Enter)
        {
            var pieces = s.Pieces.ToDictionary(k => k.Key, v => v.Value);
            pieces.Remove(move.From);
            var counters = s.Counters.WithUpdatedCounters(new List<MapPiece>());
            return new LevelStateSnapshot(s.Size, s.Floors, pieces, counters);
        }

        if (move.MovementType == MovementType.SwimRide)
        {
            var pieces = s.Pieces.ToDictionary(k => k.Key, v => v.Value);
            pieces.Remove(move.From);
            var dolphinExit = pieces.FirstOrDefault(p => p.Value == MapPiece.DolphinRideExit);
            var dolphin = pieces.FirstOrDefault(p => p.Value == MapPiece.Dolphin);
            pieces.Remove(dolphin.Key);
            pieces.Remove(dolphinExit.Key);
            pieces[dolphinExit.Key] = move.Piece;
            
            var counters = s.Counters.WithUpdatedCounters(new List<MapPiece>());
            return new LevelStateSnapshot(s.Size, s.Floors, pieces, counters);
        }
        
        return s;
    }

    public static LevelStateSnapshot GPartialMoveHeroAnimal(this LevelStateSnapshot s, TilePoint to)
    {
        var heroTile = s.Pieces.SingleOrDefault(p => p.Value == MapPiece.HeroAnimal).Key;
        var pieces = s.Pieces.ToDictionary(k => k.Key, v => v.Value);
        pieces.Remove(heroTile);
        if (!pieces.TryGetValue(to, out var pieceAtTo) || pieceAtTo != MapPiece.Barn)
            pieces[to] = MapPiece.HeroAnimal;
        return new LevelStateSnapshot(s.Size, s.Floors, pieces, s.Counters);
    }
    
    public static Dictionary<CounterType, int> WithUpdatedCounters(this Dictionary<CounterType, int> original,
        List<MapPiece> collectedPieces)
    {
        var newDict = original.ToDictionary(e => e.Key, e => e.Value);
        foreach (var ct in Enum.GetValues(typeof(CounterType)).Cast<CounterType>())
            if (!newDict.ContainsKey(ct))
                newDict[ct] = 0;
        newDict[CounterType.NumMovesMade] += 1;
        newDict[CounterType.NumTreatsCollected] += collectedPieces.Count(p => p == MapPiece.Food);
        newDict[CounterType.NumTreatsCollected] += collectedPieces.Count(p => p == MapPiece.Treat);
        return newDict;
    }
}
