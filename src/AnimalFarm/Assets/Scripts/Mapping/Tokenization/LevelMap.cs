using System;
using UnityEngine;

public sealed class LevelMap
{
    public string Name { get; }
    public MapPiece[,] FloorLayer { get; }
    public MapPiece[,] ObjectLayer { get; }

    public int Width => FloorLayer.GetLength(0);
    public int Height => FloorLayer.GetLength(1);

    public Vector2Int Size => new Vector2Int(Width, Height);
    
    public LevelMap(string name, MapPiece[,] floorLayer, MapPiece[,] objectLayer)
    {
        Name = name;
        if (floorLayer.GetLength(0) != objectLayer.GetLength(0) || floorLayer.GetLength(1) != objectLayer.GetLength(1))
            throw new ArgumentException("FloorLayer and ObjectLayer are different sizes");
        FloorLayer = floorLayer;
        ObjectLayer = objectLayer;
    }

    public TwoDimensionalIterator GetIterator() => new TwoDimensionalIterator(Width, Height);

    public LevelMap GetWithFlippedXY()
    {
        var newSize = new Vector2Int(Height, Width);
        var floorLayer = new MapPiece[newSize.x, newSize.y];
        var objectLayer = new MapPiece[newSize.x, newSize.y];
        GetIterator().ForEach(t =>
        {
            var (x, y) = t;
            floorLayer[y, x] = FloorLayer[x, y];
            objectLayer[y, x] = ObjectLayer[x, y];
        });
        return new LevelMap(Name, floorLayer, objectLayer);
    }
}
