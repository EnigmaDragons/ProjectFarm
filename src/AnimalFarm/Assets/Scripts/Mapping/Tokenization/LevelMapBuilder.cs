using System;
using UnityEngine;

public sealed class LevelMapBuilder
{
    private readonly string _name;
    private readonly MapPiece[,] _floors;
    private readonly MapPiece[,] _objects;
    
    public LevelMapBuilder(string name) : this(name, 14, 8) {}
    public LevelMapBuilder(string name, int width, int height)
    {
        _name = name;
        _floors = new MapPiece[width, height];
        _objects = new MapPiece[width, height];
    }

    public LevelMapBuilder With(TilePoint tile, MapPiece piece) => MapPieceSymbol.IsFloor(piece) ? WithFloor(tile, piece) : WithPiece(tile, piece);

    public LevelMapBuilder WithFloor(TilePoint tile, MapPiece piece)
    {
        if (!MapPieceSymbol.IsFloor(piece))
            throw new ArgumentException($"{piece} is not a floor piece.");
    
        var range = new TilePoint(_floors.GetLength(0), _floors.GetLength(1));
        if (tile.X > _floors.GetLength(0) || tile.X < 0)
            throw new ArgumentException($"{tile} is out of range {range} for {piece}");
        if (tile.Y > _floors.GetLength(1) || tile.Y < 0)
            throw new ArgumentException($"{tile} is out of range {range} for {piece}");
    
        _floors[tile.X, tile.Y] = piece;
        return this;
    }
    
    public LevelMapBuilder WithPiece(TilePoint tile, MapPiece piece)
    {
        if (!MapPieceSymbol.IsObject(piece))
            throw new ArgumentException($"{piece} is not an object piece.");
        
        var range = new TilePoint(_floors.GetLength(0), _floors.GetLength(1));
        if (tile.X > _floors.GetLength(0) || tile.X < 0)
            throw new ArgumentException($"{tile} is out of range {range} for {piece}");
        if (tile.Y > _floors.GetLength(1) || tile.Y < 0)
            throw new ArgumentException($"{tile} is out of range {range} for {piece}");
        
        _objects[tile.X, tile.Y] = piece;
        return this;
    }
    
    public LevelMap Build() => new LevelMap(_name, _floors, _objects);
}

