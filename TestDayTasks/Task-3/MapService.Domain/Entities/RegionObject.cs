using MapService.Domain.Abstractions;
using MapService.Domain.Interfaces;
using MapService.Domain.ValueObjects;

namespace MapService.Domain.Entities;

public class RegionObject:ObjectEntity, IHasName
{
    public string Name { get; set; }
    
    public RegionObject(uint id, string name, Coordinate topLeft, int width, int height)
    {
        Id = id;
        Name = name;
        TopLeft = topLeft;
        Width = width;
        Height = height;
    }
    
    
}