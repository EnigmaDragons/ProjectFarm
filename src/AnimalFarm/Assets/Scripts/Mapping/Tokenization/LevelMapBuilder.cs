using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class LevelMapBuilder
{
    private readonly string _name;
    private readonly MapPiece[,] _floors;
    private readonly MapPiece[,] _objects;

    public Vector2Int Max => new Vector2Int(MaxX, MaxY);
    public int MaxX => _floors.GetLength(0);
    public int MaxY => _floors.GetLength(1);

    public Dictionary<TilePoint, MapPiece> GetObjectsSnapshot => new TwoDimensionalIterator(MaxX, MaxY)
        .Where(xy => _objects[xy.Item1, xy.Item2] != MapPiece.Nothing)
        .ToDictionary(o => new TilePoint(o.Item1, o.Item2), o => _objects[o.Item1, o.Item2]);
    public LevelMapBuilder(string name, int width = 14, int height = 8)
    {
        _name = name;
        _floors = new MapPiece[width, height];
        _objects = new MapPiece[width, height];
    }

    public LevelMapBuilder With(TilePoint tile, MapPiece piece) => MapPieceSymbol.IsFloor(piece) ? WithFloor(tile, piece) : WithPiece(tile, piece);

    public LevelMapBuilder WithFloor(TilePoint tile) => WithFloor(tile, MapPiece.Floor);
    public LevelMapBuilder WithFloor(TilePoint tile, MapPiece piece)
    {
        if (!MapPieceSymbol.IsFloor(piece) || piece == MapPiece.Nothing)
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

        return this;
    }
    
    public LevelMapBuilder WithPiece(TilePoint tile, MapPiece piece)
    {
        if (!MapPieceSymbol.IsObject(piece))
            throw new ArgumentException($"{piece} is not an object piece.");
        
        ThrowIfNotInRange(tile, piece);
        try
        {
            _objects[tile.X, tile.Y] = piece;
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning($"{tile} exception out of range of {_floors.GetLength(0)},{_floors.GetLength(1)}");
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

    public LevelMapBuilder WithPieceAndFloor(TilePoint tile, MapPiece piece) =>
        WithPieceAndFloor(tile, piece, MapPiece.Floor);

    public LevelMapBuilder WithPieceAndFloor(TilePoint tile, MapPiece piece, MapPiece floor) =>
        WithFloor(tile, floor).WithPiece(tile, piece);

    public LevelMapBuilder MovePieceAndAddFloor(TilePoint from, TilePoint to, MapPiece piece,
        MapPiece floor = MapPiece.Floor) =>
            WithFloor(to, floor)
            .WithPiece(to, piece).WithNothing(from);

    public LevelMap Build() => new LevelMap(_name, _floors, _objects);
    
    private void ThrowIfNotInRange(TilePoint tile, MapPiece piece)
    {
        var range = new TilePoint(_floors.GetLength(0), _floors.GetLength(1));
        if (tile.X > _floors.GetLength(0) || tile.X < 0)
            throw new ArgumentException($"{tile} is out of range {range} for {piece}");
        if (tile.Y > _floors.GetLength(1) || tile.Y < 0)
            throw new ArgumentException($"{tile} is out of range {range} for {piece}");
    }
}

