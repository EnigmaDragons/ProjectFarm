using NUnit.Framework;

public class MovementTests
{
    [Test]
    public void EatFoodFromWater_CanEat()
    {
        var lb = new LevelMapBuilder("N");
        lb.WithPieceAndFloor(new TilePoint(0, 0), MapPiece.HeroAnimal, MapPiece.River);
        lb.WithPieceAndFloor(new TilePoint(1, 0), MapPiece.Food, MapPiece.Dirt);
        var levelMap = lb.Build();

        var state = levelMap.ToState();

        var possibleMoves = state.GetPossibleMoves(new TilePoint(0, 0));
        
        Assert.AreEqual(1, possibleMoves.Count);
    }

    [Test]
    public void SwimRide_IsAtEndOfAutoRide()
    {
        var lb = new LevelMapBuilder("N");
        lb.WithPieceAndFloor(new TilePoint(0, 0), MapPiece.HeroAnimal, MapPiece.Dirt);
        lb.WithPieceAndFloor(new TilePoint(1, 0), MapPiece.Dolphin, MapPiece.River);
        lb.WithPieceAndFloor(new TilePoint(2, 0), MapPiece.Nothing, MapPiece.River);
        lb.WithPieceAndFloor(new TilePoint(3, 0), MapPiece.DolphinRideExit, MapPiece.River);
        lb.WithPieceAndFloor(new TilePoint(4, 0), MapPiece.Food, MapPiece.Dirt);
        var levelMap = lb.Build();

        var state = levelMap.ToState();
        var state2 = state.ApplyMove(new LevelPlayPossibleMove
        {
            Piece = MapPiece.HeroAnimal,
            MovementType = MovementType.SwimRide,
            From = new TilePoint(0, 0),
            To = new TilePoint(1, 0),
        });

        var possibleMoves = state2.GetPossibleMoves(new TilePoint(3, 0));
        
        Assert.AreEqual(1, possibleMoves.Count);
    }
}
