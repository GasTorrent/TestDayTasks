using MapService.Domain.ValueObjects;

namespace MapService.Tests.Domain.Tests;

public class AreaTests
{
    [Fact]
    public void IsValid_ShouldReturnTrue_ForCorrectArea()
    {
        var area = new Area(new Coordinate(0, 0), new Coordinate(2, 3));
        Assert.True(area.IsValid);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_ForInvalidArea()
    {
        var area = new Area(new Coordinate(2, 3), new Coordinate(0, 0));
        Assert.False(area.IsValid);
    }

    [Fact]
    public void GetCoordinates_ShouldReturnAllCoordinates_Inclusive()
    {
        var area = new Area(new Coordinate(0, 0), new Coordinate(1, 1));
        var coords = area.GetCoordinates().ToList();

        var expected = new[]
        {
            new Coordinate(0, 0),
            new Coordinate(1, 0),
            new Coordinate(0, 1),
            new Coordinate(1, 1)
        };

        Assert.Equal(expected.Length, coords.Count);
        foreach (var coord in expected)
            Assert.Contains(coord, coords);
    }

    [Fact]
    public void GetCoordinates_InvalidArea_ShouldThrow()
    {
        var area = new Area(new Coordinate(1, 1), new Coordinate(0, 0));
        Assert.False(area.IsValid);
        Assert.Throws<InvalidOperationException>(() => area.GetCoordinates().ToList());
    }

    [Fact]
    public void SingleTileArea_ShouldReturnOneCoordinate()
    {
        var area = new Area(new Coordinate(2, 2), new Coordinate(2, 2));
        var coords = area.GetCoordinates().ToList();
        Assert.Single(coords);
        Assert.Equal(new Coordinate(2, 2), coords[0]);
    }

    [Fact]
    public void AreaWithTopLeftEqualsBottomRight_ShouldBeValid()
    {
        var area = new Area(new Coordinate(5, 5), new Coordinate(5, 5));
        Assert.True(area.IsValid);
        var coords = area.GetCoordinates().ToList();
        Assert.Single(coords);
        Assert.Equal(new Coordinate(5, 5), coords[0]);
    }
}