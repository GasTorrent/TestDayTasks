using MapService.Domain.Enums;
using MapService.Domain.ValueObjects;

namespace MapService.Domain.Entites;

public class SurfaceLayer
{
    public int Width { get; }
    public int Height { get; }
    public long TotalTiles => (long)Width * Height;

    private readonly SurfaceType[] _tiles;

    public SurfaceLayer(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive");

        Width = width;
        Height = height;
        _tiles = new SurfaceType[width * height];
    }

    public SurfaceLayer(int width, int height, IEnumerable<(Coordinate coord, SurfaceType type)> tiles) : this(width, height)
    {
        if (tiles == null)
            throw new ArgumentNullException(nameof(tiles));

        foreach (var (coord, type) in tiles)
        {
            ValidateCoordinate(coord);
            _tiles[GetIndex(coord)] = type;
        }
    }

    public SurfaceType GetTileType(Coordinate coordinate)
    {
        ValidateCoordinate(coordinate);
        return _tiles[GetIndex(coordinate)];
    }

    public void SetTileType(Coordinate coordinate, SurfaceType surfaceType)
    {
        ValidateCoordinate(coordinate);
        _tiles[GetIndex(coordinate)] = surfaceType;
    }

    public void FillArea(Area area, SurfaceType surfaceType)
    {
        ValidateArea(area);

        foreach (var coord in area.GetCoordinates())
            _tiles[GetIndex(coord)] = surfaceType;
    }

    public bool CanPlaceObjectInArea(Area area)
    {
        ValidateArea(area);

        foreach (var coord in area.GetCoordinates())
            if (!CanPlaceObject(_tiles[GetIndex(coord)]))
                return false;

        return true;
    }

    public static bool CanPlaceObject(SurfaceType surfaceType)
    {
        return surfaceType switch
        {
            SurfaceType.Plain => true,
            SurfaceType.Mountain => false,
            _ => false
        };
    }

    private int GetIndex(Coordinate coordinate) => coordinate.Y * Width + coordinate.X;

    private bool IsWithinBounds(Coordinate coordinate)
    {
        return coordinate.X >= 0 && coordinate.X < Width &&
               coordinate.Y >= 0 && coordinate.Y < Height;
    }

    private void ValidateCoordinate(Coordinate coordinate)
    {
        if (!IsWithinBounds(coordinate))
            throw new ArgumentOutOfRangeException(nameof(coordinate), $"Coordinate ({coordinate.X},{coordinate.Y}) is out of bounds");
    }

    private void ValidateArea(Area area)
    {
        if (!area.IsValid)
            throw new ArgumentException("Area is invalid");

        ValidateCoordinate(area.TopLeft);
        ValidateCoordinate(area.BottomRight);
    }
}