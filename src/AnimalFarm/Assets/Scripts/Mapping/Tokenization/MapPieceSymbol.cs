
public enum MapPiece
{
    Nothing = 0,
    Floor = 1, 
    FailsafeFloor = 2,
    RootKey = 100,
    Root = 101,
    DataCube = 102,
    Routine = 103,
    DoubleRoutine = 104,
    JumpingRoutine = 105
}

public static class MapPieceSymbol
{
    private static BidirectionalDictionary<MapPiece, string> _values = new BidirectionalDictionary<MapPiece, string>
    {
        {MapPiece.Nothing, "0"},
        {MapPiece.Floor, "1"},
        {MapPiece.FailsafeFloor, "2"},
        {MapPiece.RootKey, "K"},
        {MapPiece.Root, "X"},
        {MapPiece.DataCube, "D"},
        {MapPiece.Routine, "A"},
        {MapPiece.DoubleRoutine, "B"},
        {MapPiece.JumpingRoutine, "C"}
    };

    public static MapPiece Piece(string symbol) => _values[symbol];
    public static string Symbol(MapPiece piece) => _values[piece];

    public static bool IsFloor(MapPiece piece) => (int)piece < 100;
    public static bool IsObject(MapPiece piece) => (int)piece >= 100;
}

