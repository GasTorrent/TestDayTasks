using MapService.Domain.Abstractions;
using MapService.Domain.Enums;
using MapService.Domain.Interfaces;
using MapService.Domain.ValueObjects;

namespace MapService.Domain.Entities;

public class MapObject : ObjectEntity, IHasType<MapObjectTypes>
{
    public MapObject(uint id, Coordinate topLeft, int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width and height must be positive");

        TopLeft = topLeft;
        Width = width;
        Height = height;
        Id = id;
    }

    public MapObjectTypes Type { get; set; }
}