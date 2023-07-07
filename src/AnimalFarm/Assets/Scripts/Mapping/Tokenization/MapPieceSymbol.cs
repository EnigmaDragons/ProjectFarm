using System;
using System.Linq;

[Serializable]
public class MapPieceWithRules
{
    public MapPiece Piece;
    public ObjectRules Rules;
}

[Flags]
public enum MapPiece
{
    Nothing = 0,
    Floor = 1, 
    HeroAnimal = 2,
    Barn = 4,
    Food = 8,
    Treat = 16,
}

public static class MapPieceRules
{
    public static ObjectRules Rules(this MapPiece p)
    {
        if (p == MapPiece.HeroAnimal)
            return HeroAnimal;
        if (p == MapPiece.Food)
            return Food;
        if (p == MapPiece.Treat)
            return Treat;
        if (p == MapPiece.Floor)
            return Floor;
        if (p == MapPiece.Barn)
            return Barn;

        return new ObjectRules();
    }
    
    public static ObjectRules HeroAnimal => new ObjectRules
    {
        IsBlocking = true,
        MovementTypes = new [] { MovementType.Eat, MovementType.Enter },
    };

    public static ObjectRules Food => new ObjectRules
    {
        IsBlocking = true,
        IsJumpable = true,
        MovementTypes = new [] { MovementType.Jump },
        MovementTargetTypes = new [] { MovementType.Eat }
    };

    public static ObjectRules Treat => new ObjectRules
    {
        IsBlocking = true,
        IsCollectible = true,
        IsJumpable = true,
        MovementTypes = new [] { MovementType.Jump },
        MovementTargetTypes = new [] { MovementType.Eat }
    };

    public static ObjectRules Barn => new ObjectRules
    {
        IsBlocking = true,
        IsJumpable = false,
        MovementTargetTypes = new [] { MovementType.Enter }
    };

    public static ObjectRules Floor => new ObjectRules
    {
        IsFloor = true,
        IsWalkable = true,
    };
}

public static class MapPieceExtensions
{
    public static bool IsSelectable(this MapPiece m) => m.Rules().IsSelectable;
}

public static class MapPieceSymbol
{
    private static BidirectionalDictionary<MapPiece, string> _values = new BidirectionalDictionary<MapPiece, string>
    {
        {MapPiece.Nothing, "0"},
        {MapPiece.Floor, "1"},
        {MapPiece.HeroAnimal, "K"},
        {MapPiece.Barn, "X"},
        {MapPiece.Treat, "D"},
        {MapPiece.Food, "A"},
    };

    public static MapPiece Piece(string symbol) => _values[symbol];
    public static string Symbol(MapPiece piece) => _values[piece];

    public static bool IsFloor(MapPiece piece) => piece.Rules().IsFloor;
    public static bool IsObject(MapPiece piece) => !piece.Rules().IsFloor;

    public static bool Can(this MapPiece piece, MovementType mt) => piece.Rules().MovementTypes.Any(m => m == mt);
}

