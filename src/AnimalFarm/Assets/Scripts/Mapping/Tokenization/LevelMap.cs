using System;
using System.Linq;
using UnityEngine;

public sealed class LevelMap
{
    public string Name { get; }
    public MapPiece[,] FloorLayer { get; }
    public MapPiece[,] ObjectLayer { get; }
    public Vector2Int[] HeroPath { get; }

    public HeroAnimal Hero => (HeroAnimal)(HeroPath.Length - 1);
    public int Width => FloorLayer.GetLength(0);
    public int Height => FloorLayer.GetLength(1);

    public Vector2Int Size => new Vector2Int(Width, Height);
    
    public LevelMap(string name, MapPiece[,] floorLayer, MapPiece[,] objectLayer, Vector2Int[] heroPath)
    {
        Name = name;
        if (floorLayer.GetLength(0) != objectLayer.GetLength(0) || floorLayer.GetLength(1) != objectLayer.GetLength(1))
            throw new ArgumentException("FloorLayer and ObjectLayer are different sizes");
        FloorLayer = floorLayer;
        ObjectLayer = objectLayer;
        HeroPath = heroPath;
    }

    public TwoDimensionalIterator GetIterator() => new TwoDimensionalIterator(Width, Height);

    public LevelMap GetWithFlippedXY()
    {
        var newSize = new Vector2Int(Height, Width);
        var floorLayer = new MapPiece[newSize.x, newSize.y];
        var objectLayer = new MapPiece[newSize.x, newSize.y];
        var heroPath = HeroPath.Select(h => new Vector2Int(h.y, h.x)).ToArray();
        GetIterator().ForEach(t =>
        {
            var (x, y) = t;
            floorLayer[y, x] = FloorLayer[x, y];
            objectLayer[y, x] = ObjectLayer[x, y];
        });
        return new LevelMap(Name, floorLayer, objectLayer, heroPath);
    }
}
