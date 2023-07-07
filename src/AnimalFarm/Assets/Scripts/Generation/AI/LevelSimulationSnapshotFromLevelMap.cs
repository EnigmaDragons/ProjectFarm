using System;
using System.Collections.Generic;

public static class LevelSimulationSnapshotFromLevelMap
{
    public static LevelSimulationSnapshot Create(LevelMap map)
    {
        var iterator = new TwoDimensionalIterator(map.Width, map.Height);

        var floors = new List<TilePoint>();
        var pieces = new Dictionary<TilePoint, MapPiece>();
        var disengagedFailsafes = new List<TilePoint>();
        var oneHealthSubroutines = new List<TilePoint>();
        var twoHealthSubroutines = new List<TilePoint>();
        var iceSubroutines = new List<TilePoint>();
        var dataCubes = new List<TilePoint>();
        var root = new TilePoint(-999, -999);
        var rootKey = new TilePoint(-999, -999);
        
        iterator.ForEach(t =>
        {
            var (x, y) = t;
            var point = new TilePoint(x, y);
            if (map.FloorLayer[x, y] == MapPiece.Floor)
                floors.Add(point);
            if (map.ObjectLayer[x, y] == MapPiece.Food)
                oneHealthSubroutines.Add(point);
            if (map.ObjectLayer[x, y] == MapPiece.Treat)
                dataCubes.Add(point);
            if (map.ObjectLayer[x, y] == MapPiece.Barn)
                root = point;
            if (map.ObjectLayer[x, y] == MapPiece.HeroAnimal)
                rootKey = point;
        });
        if (root.X == -999 || rootKey.X == -999)
            throw new ArgumentException($"Map {map.Name} is missing it's Root or Root Key!");

        throw new NotImplementedException();
    }
}
