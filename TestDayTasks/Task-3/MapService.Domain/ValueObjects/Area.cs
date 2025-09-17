namespace MapService.Domain.ValueObjects;

public struct Area
{
    public Coordinate TopLeft { get; }
    public Coordinate BottomRight { get; }

    public Area(Coordinate topLeft, Coordinate bottomRight)
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    public IEnumerable<Coordinate> GetCoordinates()
    {
        if (!IsValid)
            throw new InvalidOperationException("Area is invalid");

        for (var y = TopLeft.Y; y <= BottomRight.Y; y++)
        for (var x = TopLeft.X; x <= BottomRight.X; x++)
            yield return new Coordinate(x, y);
    }

    public bool IsValid => TopLeft.X <= BottomRight.X && TopLeft.Y <= BottomRight.Y;
}