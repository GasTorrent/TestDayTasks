namespace Helpers;

public class CoordinateConverter : ICoordinateConverter
{
    private readonly int _mapWidth;
    private readonly int _mapHeight;
    private const double MinLon = -180.0;
    private const double MaxLon = 180.0;
    private const double MinLat = -85.05112878;
    private const double MaxLat = 85.05112878;

    public CoordinateConverter(
        int mapWidth,
        int mapHeight)
    {
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
    }

    public (double longitude, double latitude) ToGeoCoords(int x, int y)
    {
        return (x * 0.0001, y * 0.0001);
    }

    public (int x, int y) ToMapCoords(double longitude, double latitude)
    {
        return ((int)Math.Round(longitude / 0.0001), (int)Math.Round(latitude / 0.0001));
    }
}