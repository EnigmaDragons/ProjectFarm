using System;

public sealed class LevelMap
{
    public string Name { get; }
    public MapPiece[,] FloorLayer { get; }
    public MapPiece[,] ObjectLayer { get; }

    public int Width => FloorLayer.GetLength(0);
    public int Height => FloorLayer.GetLength(1);
    
    public LevelMap(string name, MapPiece[,] floorLayer, MapPiece[,] objectLayer)
    {
        Name = name;
        if (floorLayer.GetLength(0) != objectLayer.GetLength(0) || floorLayer.GetLength(1) != objectLayer.GetLength(1))
            throw new ArgumentException("FloorLayer and ObjectLayer are different sizes");
        FloorLayer = floorLayer;
        ObjectLayer = objectLayer;
    }

    public TwoDimensionalIterator GetIterator() => new TwoDimensionalIterator(Width, Height);
}
