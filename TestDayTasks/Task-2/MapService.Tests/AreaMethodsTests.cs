using MapService.Domain.Abstractions;
using MapService.Domain.ValueObjects;

namespace MapService.Tests;

public class AreaMethodsTests
{
    [Fact]
    public void Intersects_ShouldReturnTrue_WhenObjectIntersectsArea()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(45, 45), Width = 10, Height = 10 };
        var area = new Area(new Coordinate(40, 40), new Coordinate(50, 50));

        // Act & Assert
        Assert.True(obj.Intersects(area));
    }

    [Fact]
    public void Intersects_ShouldReturnFalse_WhenObjectDoesNotIntersectArea()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(60, 60), Width = 10, Height = 10 };
        var area = new Area(new Coordinate(40, 40), new Coordinate(50, 50));

        // Act & Assert
        Assert.False(obj.Intersects(area));
    }

    [Fact]
    public void Intersects_ShouldReturnTrue_WhenObjectTouchesAreaBoundary()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(50, 50), Width = 10, Height = 10 };
        var area = new Area(new Coordinate(40, 40), new Coordinate(50, 50));

        // Act & Assert
        Assert.True(obj.Intersects(area));
    }

    [Fact]
    public void Covers_ShouldReturnTrue_WhenCoordinateIsInsideObject()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(10, 10), Width = 20, Height = 20 };
        var coord = new Coordinate(15, 15);

        // Act & Assert
        Assert.True(obj.Covers(coord));
    }

    [Fact]
    public void Covers_ShouldReturnTrue_WhenCoordinateIsOnObjectBoundary()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(10, 10), Width = 20, Height = 20 };
        var coord = new Coordinate(10, 10);

        // Act & Assert
        Assert.True(obj.Covers(coord));
    }

    [Fact]
    public void Covers_ShouldReturnFalse_WhenCoordinateIsOutsideObject()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(10, 10), Width = 20, Height = 20 };
        var coord = new Coordinate(5, 15);

        // Act & Assert
        Assert.False(obj.Covers(coord));
    }
    
    [Fact]
    public void Intersects_ShouldReturnTrue_WhenObjectIsFullyContainedInArea()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(42, 42), Width = 5, Height = 5 };
        var area = new Area(new Coordinate(40, 40), new Coordinate(50, 50));

        // Act & Assert
        Assert.True(obj.Intersects(area));
    }

    [Fact]
    public void Intersects_ShouldReturnTrue_WhenObjectPartiallyIntersectsArea_Left()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(38, 45), Width = 5, Height = 5 };
        var area = new Area(new Coordinate(40, 40), new Coordinate(50, 50));

        // Act & Assert
        Assert.True(obj.Intersects(area));
    }

    [Fact]
    public void Intersects_ShouldReturnTrue_WhenObjectPartiallyIntersectsArea_Top()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(45, 38), Width = 5, Height = 5 };
        var area = new Area(new Coordinate(40, 40), new Coordinate(50, 50));

        // Act & Assert
        Assert.True(obj.Intersects(area));
    }

    [Fact]
    public void Covers_ShouldReturnTrue_WhenCoordinateIsOnTopLeftBoundary()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(10, 10), Width = 20, Height = 20 };
        var coord = new Coordinate(10, 10);

        // Act & Assert
        Assert.True(obj.Covers(coord));
    }

    [Fact]
    public void Covers_ShouldReturnTrue_WhenCoordinateIsOnBottomRightBoundary()
    {
        // Arrange
        var obj = new TestObject { TopLeft = new Coordinate(10, 10), Width = 20, Height = 20 };
        var coord = new Coordinate(30, 30);

        // Act & Assert
        Assert.True(obj.Covers(coord));
    }
    
    private class TestObject : ObjectEntity
    {
    }
}