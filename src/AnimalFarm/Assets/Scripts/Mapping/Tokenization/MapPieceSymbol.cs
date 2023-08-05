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
    Dirt = 1,
    HeroAnimal = 2,
    Barn = 4,
    Food = 8,
    Treat = 16,
    Dolphin = 32,
    River = 64,
    DolphinRideExit = 128,
    Seedling = 256,
    Elephant = 512,
    Fissure = 1024,
    Dino = 2048,
    HotPepper = 4096,
}

public static class MapPieceRules
{
    public static ObjectRules Rules(this MapPiece p)
    {
        if (p == MapPiece.Dirt)
            return Dirt;
        if (p == MapPiece.Seedling)
            return Seedling;
        if (p == MapPiece.River)
            return River;
        if (p == MapPiece.Fissure)
            return Fissure;
        
        if (p == MapPiece.HeroAnimal)
            return HeroAnimal;
        if (p == MapPiece.Food)
            return Food;
        if (p == MapPiece.Treat)
            return Treat;
        if (p == MapPiece.Barn)
            return Barn;
        if (p == MapPiece.Dolphin)
            return Dolphin;
        if (p == MapPiece.DolphinRideExit)
            return DolphinRideExit;
        if (p == MapPiece.Elephant)
            return Elephant;
        if (p == MapPiece.Dino)
            return Dino;
        if (p == MapPiece.HotPepper)
            return HotPepper;

        if (p != MapPiece.Nothing)
            Log.Error($"Unknown piece rules: {p}");
        return new ObjectRules();
    }
    
    public static ObjectRules HeroAnimal => new ObjectRules
    {
        IsBlocking = true,
        MovementTypes = new [] { MovementType.Eat, MovementType.Enter, MovementType.SwimRide, MovementType.Activate, MovementType.Genius },
    };

    public static ObjectRules Food => new ObjectRules
    {
        IsBlocking = true,
        IsJumpable = true,
        MovementTargetTypes = new [] { MovementType.Eat }
    };
    
    public static ObjectRules HotPepper => new ObjectRules
    {
        IsBlocking = true,
        IsJumpable = true,
        ActivationAbility = ActivationType.EatHotPepper,
        MovementTargetTypes = new [] { MovementType.Activate }
    };

    public static ObjectRules Treat => new ObjectRules
    {
        IsBlocking = true,
        IsCollectible = true,
        IsJumpable = true,
        MovementTargetTypes = new [] { MovementType.Eat }
    };

    public static ObjectRules Barn => new ObjectRules
    {
        IsBlocking = true,
        IsJumpable = false,
        MovementTargetTypes = new [] { MovementType.Enter }
    };

    public static ObjectRules Fissure => new ObjectRules
    {
        IsFloor = true,
        IsWalkable = false,
        IsJumpable = true,
    };
    
    public static ObjectRules Dirt => new ObjectRules
    {
        IsFloor = true,
        IsWalkable = true,
    };
    
    public static ObjectRules Seedling => new ObjectRules
    {
        IsFloor = true,
        IsWalkable = true,
    };

    public static ObjectRules River => new ObjectRules
    {
        IsFloor = true,
        IsWalkable = false,
        MovementTargetTypes = new[] { MovementType.SwimRide }
    };

    public static ObjectRules Dolphin => new ObjectRules
    {
        MovementTargetTypes = new[] { MovementType.SwimRide }
    };
    
    public static ObjectRules DolphinRideExit => new ObjectRules
    {
        IsBlocking = true,
        IsJumpable = true,
    };
    
    public static ObjectRules Elephant => new ObjectRules
    {
        IsBlocking = true,
        ActivationAbility = ActivationType.WaterWholeWorld,
        MovementTargetTypes = new [] { MovementType.Activate },
    };
    
    public static ObjectRules Dino => new ObjectRules
    {
        IsBlocking = true,
        ActivationAbility = ActivationType.CloseFissure,
        MovementTargetTypes = new [] { MovementType.Activate },
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
        {MapPiece.Dirt, "1"},
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

