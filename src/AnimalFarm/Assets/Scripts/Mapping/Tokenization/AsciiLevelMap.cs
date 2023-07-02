using System;
using System.Text;

public class AsciiLevelMap
{
    private readonly LevelMap _map;
    private MapPiece[,] Floor => _map.FloorLayer;
    private MapPiece[,] Objects => _map.ObjectLayer;
        
    public AsciiLevelMap(LevelMap map)
    {
        _map = map;
    }

    private static string Separator => Environment.NewLine + ">>>>>>>>>>>>>>>>>>>>" + Environment.NewLine;

    private string FloorLayer => LayerToString(Floor);
    private string ObjectLayer => LayerToString(Objects);

    private string LayerToString(MapPiece[,] layer)
    {
        var sb = new StringBuilder();
        var currentX = 0;
        foreach (var (x, y) in new TwoDimensionalIterator(_map.Width, _map.Height))
        {
            if (x != currentX)
                sb.Append(Environment.NewLine);
            sb.Append(MapPieceSymbol.Symbol(layer[x, y]));
            currentX = x;
        }
        return sb.ToString();
    }

    public override string ToString() => string.Join(Separator, FloorLayer, ObjectLayer);
}
