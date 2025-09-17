using System.Text.Json;
using Helpers;
using MapService.Application.Interfaces;
using MapService.Domain.Entities;
using MapService.Domain.ValueObjects;
using MapService.Infrastructure.Messaging.Events;
using StackExchange.Redis;

namespace MapService.Infrastructure.Repositories;

public class ObjectRepository : IObjectRepository<MapObject>
{
    private readonly IDatabase _redisDb;
    private readonly string _geoKey;
    private readonly string _dataKeyPrefix;
    private readonly IEventPublisher _eventBroker;
    private readonly ICoordinateConverter _coordinateConverter;

    public ObjectRepository(string dataKeyPrefix, IDatabase redisDb, string geoKey,
         IEventPublisher eventBroker, ICoordinateConverter coordinateConverter)
    {
        _dataKeyPrefix = dataKeyPrefix;
        _redisDb = redisDb;
        _geoKey = geoKey;
        _eventBroker = eventBroker;
        _coordinateConverter = coordinateConverter;
    }

    public async Task AddAsync(MapObject obj)
    {
       
        var (longitude, latitude) = _coordinateConverter.ToGeoCoords(obj.TopLeft.X, obj.TopLeft.Y);
        var serializedObject = JsonSerializer.Serialize(obj);

        await _redisDb.GeoAddAsync(_geoKey, new GeoEntry(longitude, latitude, obj.Id));

        await _redisDb.StringSetAsync($"{_dataKeyPrefix}{obj.Id}", serializedObject);
        _eventBroker.Publish(new ObjectCreated(obj.Id, obj.TopLeft));
    }

    public async Task UpdateAsync(MapObject obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (obj.Id == 0)
            throw new ArgumentException("Object ID cannot be zero", nameof(obj));

        var existingObjectKey = $"{_dataKeyPrefix}{obj.Id}";
        var exists = await _redisDb.KeyExistsAsync(existingObjectKey);

        if (!exists)
            throw new InvalidOperationException($"Object with ID {obj.Id} does not exist");

        await UpdateGeoIndexAsync(obj);

        var serializedObject = JsonSerializer.Serialize(obj);
        await _redisDb.StringSetAsync(existingObjectKey, serializedObject);
        _eventBroker.Publish(new ObjectUpdated(obj.Id, obj.TopLeft));
    }

    public async Task<bool> RemoveAsync(uint objectId)
    {
        await _redisDb.GeoRemoveAsync(_geoKey, objectId);
        _eventBroker.Publish(new ObjectRemoved(objectId));
        return await _redisDb.KeyDeleteAsync($"{_dataKeyPrefix}{objectId}");
    }

    public async Task<MapObject?> GetByIdAsync(uint objectId)
    {
       
        var serializedObject = await _redisDb.StringGetAsync($"{_dataKeyPrefix}{objectId}");
        if (!serializedObject.HasValue)
            return null;

        var mapObject = JsonSerializer.Deserialize<MapObject>(serializedObject);
        if (mapObject == null)
            return null;

        var geoPosition = await _redisDb.GeoPositionAsync(_geoKey, objectId);
        if (!geoPosition.HasValue) return mapObject;
        var (x, y) = _coordinateConverter.ToMapCoords(
            geoPosition.Value.Longitude,
            geoPosition.Value.Latitude);

        var expectedTopLeft = new Coordinate(x, y);
        if (mapObject.TopLeft.X != expectedTopLeft.X || mapObject.TopLeft.Y != expectedTopLeft.Y)
            mapObject.TopLeft = expectedTopLeft;

        return mapObject;
    }

    public async Task<IEnumerable<MapObject>> GetInAreaAsync(Area area)
    {
        if (!area.IsValid)
            throw new ArgumentException("Area is invalid", nameof(area));

        var centerX = (area.TopLeft.X + area.BottomRight.X) / 2;
        var centerY = (area.TopLeft.Y + area.BottomRight.Y) / 2;
        var (centerLon, centerLat) = _coordinateConverter.ToGeoCoords(centerX, centerY);

        var widthMeters = (Math.Abs(area.BottomRight.X - area.TopLeft.X) + 1) * 200;
        var heightMeters = (Math.Abs(area.BottomRight.Y - area.TopLeft.Y) + 1) * 200;

        var geoResults = await _redisDb.GeoSearchAsync(
            _geoKey,
            centerLon,
            centerLat,
            new GeoSearchBox(widthMeters, heightMeters)
        );

        var foundObjects = new List<MapObject>();
        foreach (var result in geoResults)
        {
            var obj = await GetByIdAsync((uint)result.Member);
            if (obj?.Intersects(area) == true)
                foundObjects.Add(obj);
        }

        return foundObjects;
    }



    public async Task<MapObject?> GetByCoordinateAsync(Coordinate coordinate)
    {
       
        var (centerLon, centerLat) = _coordinateConverter.ToGeoCoords(coordinate.X, coordinate.Y);
        
        var geoResults = await _redisDb.GeoSearchAsync(
            _geoKey,
            centerLon,
            centerLat,
            new GeoSearchBox(1, 1, GeoUnit.Kilometers)
        );
    
        foreach (var result in geoResults)
        {
            var obj = await GetByIdAsync((uint)result.Member);
            if (obj != null && obj.Covers(coordinate)) 
            {
                return obj;
            }
        }

        return null;
    }

    private async Task UpdateGeoIndexAsync(MapObject obj)
    {
       
        var (longitude, latitude) = _coordinateConverter.ToGeoCoords(obj.TopLeft.X, obj.TopLeft.Y);

        await _redisDb.GeoRemoveAsync(_geoKey, obj.Id);
        await _redisDb.GeoAddAsync(_geoKey, new GeoEntry(longitude, latitude, obj.Id));
    }
    
    
}