using MapService.Domain.ValueObjects;

namespace MapService.Tests.Domain.Tests;

public class CoordinateTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        var coord = new Coordinate(3, 5);
        Assert.Equal(3, coord.X);
        Assert.Equal(5, coord.Y);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(-5, -2)]
    public void Constructor_NegativeValues_ShouldThrow(int x, int y)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Coordinate(x, y));
    }

    [Fact]
    public void TwoCoordinatesWithSameValues_ShouldBeEqual()
    {
        var c1 = new Coordinate(2, 3);
        var c2 = new Coordinate(2, 3);
        Assert.Equal(c1.X, c2.X);
        Assert.Equal(c1.Y, c2.Y);
    }

    [Fact]
    public void TwoCoordinatesWithDifferentValues_ShouldNotBeEqual()
    {
        var c1 = new Coordinate(2, 3);
        var c2 = new Coordinate(3, 2);
        Assert.NotEqual(c1.X, c2.X);
        Assert.NotEqual(c1.Y, c2.Y);
    }
}