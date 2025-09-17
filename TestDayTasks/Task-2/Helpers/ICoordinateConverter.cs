namespace Helpers;

public interface ICoordinateConverter
{
    (double longitude, double latitude) ToGeoCoords(int x, int y);
    (int x, int y) ToMapCoords(double longitude, double latitude);
}