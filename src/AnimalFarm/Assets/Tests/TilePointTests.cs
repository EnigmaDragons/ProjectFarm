using System.Linq;
using NUnit.Framework;

public class TilePointTests
{
    [Test]
    public void InBetweenSameSpaces_ReturnsNoTiles() 
        => Assert.IsEmpty(new TilePoint(0, 0).InBetween(new TilePoint(0, 0)));

    [Test]
    public void InBetweenAdjacentSpaces_ReturnsNoTiles()
        => Assert.IsEmpty(new TilePoint(0, 0).InBetween(new TilePoint(0, 1)));

    [Test]
    public void InBetweenTwoSpacesApart_ReturnInBetweenSpace()
    {
        var tilesBetween = new TilePoint(0, 0).InBetween(new TilePoint(0, 2));
        Assert.AreEqual(1, tilesBetween.Count);
        Assert.AreEqual(tilesBetween[0], new TilePoint(0, 1));
    }

    [Test]
    public void InBetweenCube_GivesMiddleSquares()
    {
        var tilesBetween = new TilePoint(0, 0).InBetween(new TilePoint(3, 3));
        Assert.AreEqual(4, tilesBetween.Count);
        Assert.True(tilesBetween.Any(x => x.X == 1 && x.Y == 1));
        Assert.True(tilesBetween.Any(x => x.X == 1 && x.Y == 2));
        Assert.True(tilesBetween.Any(x => x.X == 2 && x.Y == 1));
        Assert.True(tilesBetween.Any(x => x.X == 2 && x.Y == 2));
    }
}
