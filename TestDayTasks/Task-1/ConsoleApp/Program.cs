using MapService.Domain.Entites;
using MapService.Domain.Enums;
using MapService.Domain.ValueObjects;

var map = new SurfaceLayer(100, 100);

var singleCoordinate = new Coordinate(5, 5);
map.SetTileType(singleCoordinate, SurfaceType.Mountain);
Console.WriteLine($"Tile at {singleCoordinate.X},{singleCoordinate.Y} set to Mountain");

var mountainBlock = new Area(new Coordinate(10, 10), new Coordinate(15, 15));
map.FillArea(mountainBlock, SurfaceType.Mountain);
Console.WriteLine($"Filled area ({mountainBlock.TopLeft.X},{mountainBlock.TopLeft.Y}) - ({mountainBlock.BottomRight.X},{mountainBlock.BottomRight.Y}) with Mountain");

var typeAtFiveFive = map.GetTileType(singleCoordinate);
Console.WriteLine($"Tile at {singleCoordinate.X},{singleCoordinate.Y} is {typeAtFiveFive}");

var plainArea = new Area(new Coordinate(0, 0), new Coordinate(4, 4));
Console.WriteLine($"Can place object in plain area? {map.CanPlaceObjectInArea(plainArea)}");

var mountainArea = new Area(new Coordinate(11, 11), new Coordinate(12, 12));
Console.WriteLine($"Can place object in mountain area? {map.CanPlaceObjectInArea(mountainArea)}");

var mixedArea = new Area(new Coordinate(4, 4), new Coordinate(6, 6));
Console.WriteLine($"Can place object in mixed area? {map.CanPlaceObjectInArea(mixedArea)}");

var initialTiles = new[]
{
    (new Coordinate(0, 0), SurfaceType.Mountain),
    (new Coordinate(0, 1), SurfaceType.Mountain),
    (new Coordinate(1, 0), SurfaceType.Plain)
};
var prefilledMap = new SurfaceLayer(5, 5, initialTiles);
Console.WriteLine("Prefilled map created with some initial tiles");

foreach (var coord in new Area(new Coordinate(0, 0), new Coordinate(1, 1)).GetCoordinates())
{
    var tileType = prefilledMap.GetTileType(coord);
    Console.WriteLine($"Tile at {coord.X},{coord.Y} is {tileType}");
}

try
{
    map.SetTileType(new Coordinate(20, 20), SurfaceType.Plain);
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Caught expected exception: {ex.Message}");
}
