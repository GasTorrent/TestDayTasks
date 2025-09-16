namespace MapService.Domain.ValueObjects;

public struct Coordinate
{
    public int X { get; }
    public int Y { get; }

    public Coordinate(int x, int y)
    {
        if (x < 0 || y < 0)
            throw new ArgumentOutOfRangeException(nameof(x), "Coordinates must be non-negative");

        X = x;
        Y = y;
    }
}