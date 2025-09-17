using MapService.Domain.Abstractions;
using MapService.Domain.ValueObjects;

namespace MapService.Domain.Entities;

public class RegionLayer:LayerEntity<RegionObject>
{
    private readonly ushort[,] _regionMap;
    private readonly Dictionary<uint, RegionObject> _regions = new();
    
    public RegionLayer(uint width, uint height)
    {
        Width = width;
        Height = height;
        Objects = new List<RegionObject>();
        _regionMap = new ushort[width, height];
    }

    public void AddRegion(RegionObject region)
    {
        Objects.Add(region);
        _regions[region.Id] = region;

        for (var x = region.TopLeft.X; x < region.TopLeft.X + region.Width; x++)
        for (var y = region.TopLeft.Y; y < region.TopLeft.Y + region.Height; y++)
        {
            _regionMap[x, y] = (ushort)region.Id;
        }
    }

    public uint GetRegionId(int x, int y) => _regionMap[x, y];

    public RegionObject? GetRegion(uint id) =>
        _regions.TryGetValue(id, out var region) ? region : null;

    public bool ContainsTile(uint regionId, Coordinate coord)
    {
        var region = GetRegion(regionId);
        return region != null && region.Covers(coord);
    }

    public IEnumerable<RegionObject> GetRegionsInArea(Area area)
    {
        var ids = new HashSet<uint>();

        for (var x = area.TopLeft.X; x <= area.BottomRight.X; x++)
        for (var y = area.TopLeft.Y; y <= area.BottomRight.Y; y++)
        {
            ids.Add(_regionMap[x, y]);
        }

        foreach (var id in ids)
        {
            if (_regions.TryGetValue(id, out var region))
                yield return region;
        }
    }
    
    public void GenerateRegions(uint regionWidth, uint regionHeight)
    {
        uint id = 1;
        for (var y = 0; y < Height; y += (int)regionHeight)
        for (var x = 0; x < Width; x += (int)regionWidth)
        {
            var region = new RegionObject(
                id,
                $"Region {id}",
                new Coordinate(x, y),
                (int)regionWidth,
                (int)regionHeight
            );

            AddRegion(region);
            id++;
        }
    }
}