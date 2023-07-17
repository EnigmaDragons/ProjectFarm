using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class LevelMapBuilder
{
    private readonly string _name;
    private readonly MapPiece[,] _floors;
    private readonly MapPiece[,] _objects;
    private readonly HashSet<MapPiece> _nonEffectivePieces;

    public Vector2Int Max => new Vector2Int(MaxX, MaxY);
    public int MaxX => _floors.GetLength(0);
    public int MaxY => _floors.GetLength(1);

    public int EffectiveMinX { get; private set; } = 99;
    public int EffectiveMaxX { get; private set; } = 0;
    public int EffectiveMinY { get; private set; } = 99;
    public int EffectiveMaxY { get; private set; } = 0;
    
    public int EffectiveWidth => (EffectiveMaxX - EffectiveMinX) + 1;
    public int EffectiveHeight => (EffectiveMaxY - EffectiveMinY) + 1;

    public Dictionary<TilePoint, MapPiece> GetObjectsSnapshot => new TwoDimensionalIterator(MaxX, MaxY)
        .Where(xy => _objects[xy.Item1, xy.Item2] != MapPiece.Nothing)
        .ToDictionary(o => new TilePoint(o.Item1, o.Item2), o => _objects[o.Item1, o.Item2]);
    public LevelMapBuilder(string name, int width = 14, int height = 8, HashSet<MapPiece> excludedPieceFromEffectiveCalc = null)
    {
        _name = name;
        _floors = new MapPiece[width, height];
        _objects = new MapPiece[width, height];
        _nonEffectivePieces = excludedPieceFromEffectiveCalc ?? new HashSet<MapPiece>();
    }
    
    public LevelMapBuilder With(TilePoint tile, MapPiece piece) => MapPieceSymbol.IsFloor(piece) ? WithFloor(tile, piece) : WithPiece(tile, piece);

    public LevelMapBuilder WithFloor(TilePoint tile, MapPiece piece)
    {
        if (!MapPieceSymbol.IsFloor(piece) && piece != MapPiece.Nothing)
            throw new ArgumentException($"{piece} is not a floor piece.");
    
        ThrowIfNotInRange(tile, piece);
        try
        {
            _floors[tile.X, tile.Y] = piece;
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning($"{tile} exception out of range of {_floors.GetLength(0)},{_floors.GetLength(1)}");
        }

        UpdateEffectiveValues(tile, piece);
        return this;
    }
    
    public LevelMapBuilder WithPiece(TilePoint tile, MapPiece piece)
    {
        if (!MapPieceSymbol.IsObject(piece) && piece != MapPiece.Nothing)
            throw new ArgumentException($"{piece} is not an object piece.");
        
        ThrowIfNotInRange(tile, piece);
        try
        {
            _objects[tile.X, tile.Y] = piece;
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning($"{tile} exception out of range of {_objects.GetLength(0)},{_objects.GetLength(1)}");
        }
        
        UpdateEffectiveValues(tile, piece);
        return this;
    }

    private LevelMapBuilder TeleportPieceAndFloor(TilePoint from, TilePoint to)
    {
        try
        {
            if (_floors[from.X, from.Y] == MapPiece.Nothing && _objects[from.X, from.Y] == MapPiece.Nothing)
                return this;
            ThrowIfNotInRange(to, MapPiece.Nothing);
            if (_floors[to.X, to.Y] != MapPiece.Nothing)
                throw new ArgumentException($"{to} already has a floor piece.");
            if (_objects[to.X, to.Y] != MapPiece.Nothing)
                throw new ArgumentException($"{from} already has an object piece.");

            _floors[to.X, to.Y] = _floors[from.X, from.Y];
            _objects[to.X, to.Y] = _objects[from.X, from.Y];
            _floors[from.X, from.Y] = MapPiece.Nothing;
            _objects[from.X, from.Y] = MapPiece.Nothing;
            UpdateEffectiveValues(to, _objects[to.X, to.Y]);
            UpdateEffectiveValues(to, _floors[to.X, to.Y]);
            Log.SInfo(LogScopes.Gen, $"Teleported whole tile {from} -> {to}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"{to} exception out of range of {_objects.GetLength(0)},{_objects.GetLength(1)}");
        }

        return this;
    }
    
    private LevelMapBuilder WithNothing(TilePoint tile)
    {
        var piece = MapPiece.Nothing;
        ThrowIfNotInRange(tile, piece);
        _objects[tile.X, tile.Y] = piece;
        return this;
    }

    public LevelMapBuilder WithPieceAndFloor(TilePoint tile, MapPiece piece, MapPiece floor) =>
        WithFloor(tile, floor).WithPiece(tile, piece);

    public LevelMapBuilder WithMovedPieceAndAddedFloorIfMissing(TilePoint from, TilePoint to, MapPiece piece,
        MapPiece floor)
    {
        if (_floors[to.X, to.Y] == MapPiece.Nothing)
            WithFloor(to, floor);
        return WithPiece(to, piece).WithNothing(from);
    }

    public LevelMapBuilder WithShifted(Func<TilePoint, bool> shouldShift, Vector2Int offset)
    {
        new TwoDimensionalIterator(Max.x, Max.y)
            .Where(xy => shouldShift(new TilePoint(xy.Item1, xy.Item2)))
            .ForEach(xy => TeleportPieceAndFloor(new TilePoint(xy.Item1, xy.Item2), new TilePoint(xy.Item1 + offset.x, xy.Item2 + offset.y)));
        
        return this;
    }

    public LevelMap Build() => new LevelMap(_name, _floors, _objects);
    
    public LevelMap BuildTrimmed()
    {
        var finalFloors = new MapPiece[EffectiveWidth, EffectiveHeight];
        var finalObjects = new MapPiece[EffectiveWidth, EffectiveHeight];
            new TwoDimensionalIterator(EffectiveWidth, EffectiveHeight)
                .ForEach(xy =>
                {
                    var src = new TilePoint(xy.Item1 + EffectiveMinX, xy.Item2 + EffectiveMinY);
                    try
                    {
                        finalFloors[xy.Item1, xy.Item2] = _floors[xy.Item1 + EffectiveMinX, xy.Item2 + EffectiveMinY];
                        finalObjects[xy.Item1, xy.Item2] = _objects[xy.Item1 + EffectiveMinX, xy.Item2 + EffectiveMinY];
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Trimmed Dest {xy} is out of range of {_floors.GetLength(0)},{_floors.GetLength(1)}. Original {src}. EffectiveMinX {EffectiveMinX} EffectiveMinY {EffectiveMinY}");
                    }
                });
        return new LevelMap(_name, finalFloors, finalObjects);
    }

    private void ThrowIfNotInRange(TilePoint tile, MapPiece piece)
    {
        var range = new TilePoint(_floors.GetLength(0), _floors.GetLength(1));
        if (tile.X > _floors.GetLength(0) || tile.X < 0)
            throw new ArgumentException($"{tile} is out of range {range} for {piece}");
        if (tile.Y > _floors.GetLength(1) || tile.Y < 0)
            throw new ArgumentException($"{tile} is out of range {range} for {piece}");
    }

    private void UpdateEffectiveValues(TilePoint newTile, MapPiece piece)
    {
        if (piece == MapPiece.Nothing || _nonEffectivePieces.Contains(piece))
            return;
        
        EffectiveMinX = Math.Min(EffectiveMinX, newTile.X);
        EffectiveMaxX = Math.Max(EffectiveMaxX, newTile.X);
        EffectiveMinY = Math.Min(EffectiveMinY, newTile.Y);
        EffectiveMaxY = Math.Max(EffectiveMaxY, newTile.Y);
    }
}
