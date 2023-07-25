using System;
using System.Linq;
using System.Text;
using UnityEngine;

public class TokenizedLevelMap
{
    private readonly LevelMap _map;
    private MapPiece[,] Floor => _map.FloorLayer;
    private MapPiece[,] Objects => _map.ObjectLayer;
    private Vector2Int[] HeroPath => _map.HeroPath;
        
    public TokenizedLevelMap(LevelMap map)
    {
        _map = map;
    }

    private static string Separator => ">";
    private static string Header => $"EnigmaFarmMap";
    private string Size => $"Size[{_map.Width},{_map.Height}]";
    private string FloorLayer => LayerToString(Floor);
    private string ObjectLayer => LayerToString(Objects);
    private string HeroCode => string.Join("-", HeroPath.Select(h => $"{h.x},{h.y}"));

    private string LayerToString(MapPiece[,] layer)
    {
        var sb = new StringBuilder();
        new TwoDimensionalIterator(_map.Width, _map.Height)
            .ForEach(p => sb.Append(MapPieceSymbol.Symbol(layer[p.Item1, p.Item2])));
        return sb.ToString();
    }

    public override string ToString() => string.Join(Separator, Header, _map.Name, Size, FloorLayer, ObjectLayer, HeroCode);

    public static LevelMap FromString(string token)
    {
        var parts = token.Split(Separator[0]);
        if (!parts[0].Equals(Header))
            throw new ArgumentException("Invalid Token");

        var name = parts[1];
        var size = parts[2].Substring(4).TrimStart('[').TrimEnd(']').Split(',');
        var width = int.Parse(size[0]);
        var height = int.Parse(size[1]);
        var levelMapBuilder = new LevelMapBuilder(name, width, height);

        var floor = parts[2];
        new TwoDimensionalIterator(width, height)
            .ForEach(p => levelMapBuilder.With(
                new TilePoint(p.Item1, p.Item2), 
                MapPieceSymbol.Piece(floor[p.Item1 + p.Item2 * width].ToString())));

        var objects = parts[3];
        new TwoDimensionalIterator(width, height)
            .ForEach(p => levelMapBuilder.With(
                new TilePoint(p.Item1, p.Item2), 
                MapPieceSymbol.Piece(objects[p.Item1 + p.Item2 * width].ToString())));

        var heroPathStr = parts[4];
        var heroPath = heroPathStr.Split('-')
            .Select(h => h.Split(','))
            .Select(h => new Vector2Int(int.Parse(h[0]), int.Parse(h[1])))
            .ToArray();
        levelMapBuilder.WithHeroPath(heroPath);
        
        return levelMapBuilder.Build();
    }
} 
