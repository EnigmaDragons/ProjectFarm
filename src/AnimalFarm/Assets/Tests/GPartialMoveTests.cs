using NUnit.Framework;

public class GPartialMoveTests
{
    [Test]
    public void Hero_FourAdjacents()
    {
        var tp = new TilePoint(1, 1);
        var lb = new LevelMapBuilder("", 5, 5);
        lb.WithPieceAndFloor(tp, MapPiece.HeroAnimal, MapPiece.Dirt);
        var l = lb.Build().ToState();

        var moves = l.GetGPartialMoves(tp, true);
        
        CollectionAssert.AreEquivalent(new [] { new TilePoint(0, 1), new TilePoint(1, 0), new TilePoint(2, 1), new TilePoint(1, 2) }, moves);
    }
    
    [Test]
    public void Hero_NoWaterWalking()
    {
        var tp = new TilePoint(1, 1);
        var lb = new LevelMapBuilder("", 5, 5);
        lb.WithPieceAndFloor(tp, MapPiece.HeroAnimal, MapPiece.Dirt);
        lb.WithPieceAndFloor(new TilePoint(2, 1), MapPiece.Nothing, MapPiece.River);
        var l = lb.Build().ToState();

        var moves = l.GetGPartialMoves(tp, true);
        
        CollectionAssert.AreEquivalent(new [] { new TilePoint(0, 1), new TilePoint(1, 0), new TilePoint(1, 2) }, moves);
    }
    
    [Test]
    public void Hero_UpdatedAfterHeroMove_FourAdjacents()
    {
        var tp = new TilePoint(1, 1);
        var lb = new LevelMapBuilder("", 5, 5);
        lb.WithPieceAndFloor(tp, MapPiece.HeroAnimal, MapPiece.Dirt);
        
        var l = lb.Build().ToState();
        var tp2 = new TilePoint(2, 1);
        var l2 = l.GPartialMoveHeroAnimal(tp2);
        
        var moves = l2.GetGPartialMoves(tp2, true);
        
        CollectionAssert.AreEquivalent(new [] { new TilePoint(1, 1), new TilePoint(2, 0), new TilePoint(3, 1), new TilePoint(2, 2) }, moves);
    }
    
    [Test]
    public void Hero_MovedToBarn_NoHero()
    {
        var tp = new TilePoint(1, 1);
        var tp2 = new TilePoint(2, 1);
        var lb = new LevelMapBuilder("", 5, 5);
        lb.WithPieceAndFloor(tp, MapPiece.HeroAnimal, MapPiece.Dirt);
        lb.WithPieceAndFloor(tp2, MapPiece.Barn, MapPiece.Dirt);
        
        var l2 = lb.Build().ToState().GPartialMoveHeroAnimal(tp2);
        
        Assert.IsTrue(l2.Pieces.None(p => p.Value == MapPiece.HeroAnimal));
    }
}
