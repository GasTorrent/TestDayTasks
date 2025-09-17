using MapService.Domain.Abstractions;
using MapService.Domain.Enums;

namespace MapService.Domain.Entities;

public class Map : Entity
{
    public uint Width { get; set; }
    public uint Height { get; set; }
    public ICollection<LayerEntity<MapObject>> Layers { get; set; }
}