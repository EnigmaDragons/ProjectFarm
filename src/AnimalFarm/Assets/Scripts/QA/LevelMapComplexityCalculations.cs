using System.Linq;
using UnityEngine;

public static class LevelMapComplexityCalculations
{
    public static BasicPuzzleComplexityStats BasicComplexityStats(this LevelMap map) 
        => new BasicPuzzleComplexityStats(map.Name, map.NumSpaces(), map.NumJumps(), map.PieceComplexity());

    public static int NumSpaces(this LevelMap map) 
        => map
            .GetIterator()
            .Select(l => map.FloorLayer[l.Item1, l.Item2])
            .Count(x => x != MapPiece.Nothing);

    public static int NumJumps(this LevelMap map)
        => map
            .GetIterator()
            .Select(l => map.ObjectLayer[l.Item1, l.Item2])
            .Where(m => m != MapPiece.Nothing)
            .Sum(p => kNumJumps[p]);

    public static int PieceComplexity(this LevelMap map)
        => map
            .GetIterator()
            .SelectMany(l => new[] {map.ObjectLayer[l.Item1, l.Item2], map.FloorLayer[l.Item1, l.Item2]})
            .Where(m => m != MapPiece.Nothing)
            .Sum(p => kPieceComplexity[p]);
    
    private static readonly DictionaryWithDefault<MapPiece, int> kNumJumps = new DictionaryWithDefault<MapPiece, int>(0)
    {
        { MapPiece.Food, 1 },
    };
    
    private static readonly DictionaryWithDefault<MapPiece, int> kPieceComplexity = new DictionaryWithDefault<MapPiece, int>(0)
    {
        { MapPiece.Floor, 0 },
        { MapPiece.Barn, 1 },
        { MapPiece.StarFood, 2 },
        { MapPiece.HeroAnimal, 1 },
        { MapPiece.Food, 1 },
    };
}