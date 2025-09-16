using MapService.Domain.Entites;
using MapService.Domain.Enums;
using MapService.Domain.ValueObjects;

namespace MapService.Tests.Domain.Tests;

public class SurfaceLayerTests
{
    [Fact]
    public void CreateEmptyLayer_ShouldHaveCorrectSize()
    {
        var layer = new SurfaceLayer(10, 20);
        Assert.Equal(10, layer.Width);
        Assert.Equal(20, layer.Height);
        Assert.Equal(200, layer.TotalTiles);
    }

    [Fact]
    public void CreateLayerFromTiles_ShouldSetTilesCorrectly()
    {
        var tiles = new List<(Coordinate, SurfaceType)>
        {
            (new Coordinate(0, 0), SurfaceType.Plain),
            (new Coordinate(1, 0), SurfaceType.Mountain),
            (new Coordinate(0, 1), SurfaceType.Mountain)
        };
        var layer = new SurfaceLayer(2, 2, tiles);

        Assert.Equal(SurfaceType.Plain, layer.GetTileType(new Coordinate(0, 0)));
        Assert.Equal(SurfaceType.Mountain, layer.GetTileType(new Coordinate(1, 0)));
        Assert.Equal(SurfaceType.Mountain, layer.GetTileType(new Coordinate(0, 1)));
        Assert.Equal(SurfaceType.Plain, layer.GetTileType(new Coordinate(1, 1))); // default
    }

    [Fact]
    public void GetTileType_OutOfBounds_ShouldThrow()
    {
        var layer = new SurfaceLayer(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => layer.GetTileType(new Coordinate(2, 0)));
        Assert.Throws<ArgumentOutOfRangeException>(() => layer.GetTileType(new Coordinate(0, 2)));
        Assert.Throws<ArgumentOutOfRangeException>(() => layer.GetTileType(new Coordinate(-1, 0)));
    }

    [Fact]
    public void SetTileType_ShouldUpdateTile()
    {
        var layer = new SurfaceLayer(2, 2);
        var coord = new Coordinate(1, 1);
        layer.SetTileType(coord, SurfaceType.Mountain);
        Assert.Equal(SurfaceType.Mountain, layer.GetTileType(coord));
    }

    [Fact]
    public void FillArea_ShouldSetAllTiles()
    {
        var layer = new SurfaceLayer(3, 3);
        var area = new Area(new Coordinate(0, 0), new Coordinate(1, 1));
        layer.FillArea(area, SurfaceType.Mountain);

        for (int y = 0; y <= 1; y++)
        for (int x = 0; x <= 1; x++)
            Assert.Equal(SurfaceType.Mountain, layer.GetTileType(new Coordinate(x, y)));

        Assert.Equal(SurfaceType.Plain, layer.GetTileType(new Coordinate(2, 2)));
    }

    [Fact]
    public void CanPlaceObjectInArea_ShouldReturnCorrectly()
    {
        var layer = new SurfaceLayer(2, 2);
        layer.SetTileType(new Coordinate(0, 0), SurfaceType.Plain);
        layer.SetTileType(new Coordinate(0, 1), SurfaceType.Mountain);
        layer.SetTileType(new Coordinate(1, 0), SurfaceType.Plain);
        layer.SetTileType(new Coordinate(1, 1), SurfaceType.Plain);

        var area1 = new Area(new Coordinate(0, 0), new Coordinate(1, 0));
        var area2 = new Area(new Coordinate(0, 0), new Coordinate(0, 1));

        Assert.True(layer.CanPlaceObjectInArea(area1)); 
        Assert.False(layer.CanPlaceObjectInArea(area2)); 
    }

    [Fact]
    public void Constructor_NullTiles_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => new SurfaceLayer(2, 2, null));
    }

    [Fact]
    public void FillArea_InvalidArea_ShouldThrow()
    {
        var layer = new SurfaceLayer(2, 2);
        var area = new Area(new Coordinate(1, 1), new Coordinate(0, 0)); // invalid
        Assert.Throws<ArgumentException>(() => layer.FillArea(area, SurfaceType.Plain));
    }

    [Fact]
    public void CanPlaceObjectInArea_InvalidArea_ShouldThrow()
    {
        var layer = new SurfaceLayer(2, 2);
        var area = new Area(new Coordinate(1, 1), new Coordinate(0, 0)); // invalid
        Assert.Throws<ArgumentException>(() => layer.CanPlaceObjectInArea(area));
    }

    [Fact]
    public void Constructor_TileOutOfBounds_ShouldThrow()
    {
        var tiles = new List<(Coordinate, SurfaceType)>
        {
            (new Coordinate(2, 0), SurfaceType.Plain)
        };
        Assert.Throws<ArgumentOutOfRangeException>(() => new SurfaceLayer(2, 2, tiles));
    }
}