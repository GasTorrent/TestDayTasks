using Helpers;
using MapService.Domain.Entities;
using MapService.Domain.ValueObjects;
using MapService.Infrastructure.Messaging.Events;
using MapService.Infrastructure.Messaging.Publishers;
using MapService.Infrastructure.Repositories;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace MapService.Tests;

public class ObjectRepositoryTests : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer;
    private ConnectionMultiplexer _redis;
    private ObjectRepository _repository;
    private SimpleEventBroker _eventBroker;
    
    public ObjectRepositoryTests()
    {
        _redisContainer = new RedisBuilder()
            .WithImage("redis:latest")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
        _redis = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
        _eventBroker = new SimpleEventBroker();
        _repository = new ObjectRepository("mapobj:", _redis.GetDatabase(), "geo:map", _eventBroker,new CoordinateConverter(1000,1000));
    }

    public async Task DisposeAsync()
    {
        await _redisContainer.StopAsync();
        await _redis.DisposeAsync();
    }

    [Fact]
    public async Task Add_Get_Remove_Object_Should_Work()
    {
        var obj = new MapObject(1,
            new Coordinate(2, 2),
            5,
            5);

        await _repository.AddAsync(obj);

        var fetched = await _repository.GetByIdAsync(1);
        Assert.NotNull(fetched);
        Assert.Equal(obj.Id, fetched!.Id);

        var byCoord = await _repository.GetByCoordinateAsync(new Coordinate(2, 2));
        Assert.NotNull(byCoord);
        Assert.Equal(obj.Id, byCoord!.Id);

        var removed = await _repository.RemoveAsync(1);
        Assert.True(removed);

        var afterRemove = await _repository.GetByIdAsync(1);
        Assert.Null(afterRemove);
    }

    [Fact]
    public async Task GetInArea_Should_Return_Object()
    {
        var obj = new MapObject(3,
            new Coordinate(52, 52),
            5,
            5);
        await _repository.AddAsync(obj);

        var area = new Area(new Coordinate(50, 50), new Coordinate(55, 55));
        var results = await _repository.GetInAreaAsync(area);
        Assert.Contains(results, o => o.Id == obj.Id);
    }

    [Fact]
    public async Task Add_Get_Remove_Object_ByCoord_Should_Work()
    {
        var obj = new MapObject(7,
            new Coordinate(80, 80),
            10,
            10);

        await _repository.AddAsync(obj);

        var byCoord = await _repository.GetByCoordinateAsync(new Coordinate(80, 80));
        Assert.NotNull(byCoord);
        Assert.Equal(obj.Id, byCoord!.Id);

        var removed = await _repository.RemoveAsync(7);
        Assert.True(removed);

        var afterRemoveByCoord = await _repository.GetByCoordinateAsync(new Coordinate(85, 85));
        Assert.Null(afterRemoveByCoord);
    }

    [Fact]
    public async Task GetInArea_ShouldReturnMultipleObjects_WhenMultipleObjectsExistInArea()
    {
        // Arrange
        var obj1 = new MapObject(8, new Coordinate(10, 10), 5, 5);
        var obj2 = new MapObject(9, new Coordinate(12, 12), 5, 5);
        var obj3 = new MapObject(10, new Coordinate(50, 50), 5, 5);

        await _repository.AddAsync(obj1);
        await _repository.AddAsync(obj2);
        await _repository.AddAsync(obj3);

        var area = new Area(new Coordinate(8, 8), new Coordinate(15, 15));

        // Act
        var results = await _repository.GetInAreaAsync(area);

        // Assert
        Assert.Equal(2, results.Count());
        Assert.Contains(results, o => o.Id == obj1.Id);
        Assert.Contains(results, o => o.Id == obj2.Id);
        Assert.DoesNotContain(results, o => o.Id == obj3.Id);
    }

    [Fact]
    public async Task GetInArea_ShouldReturnEmptyList_WhenNoObjectsIntersectArea()
    {
        // Arrange
        var obj = new MapObject(11, new Coordinate(100, 100), 5, 5);
        await _repository.AddAsync(obj);

        var area = new Area(new Coordinate(10, 10), new Coordinate(20, 20));

        // Act
        var results = await _repository.GetInAreaAsync(area);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetInArea_ShouldReturnObject_WhenObjectPartiallyIntersectsArea()
    {
        // Arrange
        var obj = new MapObject(12, new Coordinate(48, 48), 5, 5);
        await _repository.AddAsync(obj);

        var area = new Area(new Coordinate(50, 50), new Coordinate(60, 60));

        // Act
        var results = await _repository.GetInAreaAsync(area);

        // Assert
        Assert.Single(results);
        Assert.Equal(obj.Id, results.First().Id);
    }

    [Fact]
    public async Task GetInArea_ShouldReturnObject_WhenObjectTouchesAreaBoundary()
    {
        // Arrange
        var obj = new MapObject(13, new Coordinate(55, 55), 5, 5);
        await _repository.AddAsync(obj);

        var area = new Area(new Coordinate(50, 50), new Coordinate(55, 55));

        // Act
        var results = await _repository.GetInAreaAsync(area);

        // Assert
        Assert.Single(results);
        Assert.Equal(obj.Id, results.First().Id);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Object_And_Publish_Event()
    {
        // Arrange
        var testObject = new MapObject(4, new Coordinate(100, 200), 5, 5);
        ObjectCreated? receivedEvent = null;
        _eventBroker.Subscribe<ObjectCreated>(e => receivedEvent = e);

        // Act
        await _repository.AddAsync(testObject);
        var fetched = await _repository.GetByIdAsync(testObject.Id);

        // Assert
        Assert.NotNull(fetched);
        Assert.Equal(testObject.Id, fetched.Id);
        Assert.NotNull(receivedEvent);
        Assert.Equal(testObject.Id, receivedEvent.EntityId);
        Assert.Equal(testObject.TopLeft, receivedEvent.ObjectCoordinates);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Object_And_Publish_Event()
    {
        // Arrange
        var initialObject = new MapObject(5, new Coordinate(50, 50), 5, 5);
        await _repository.AddAsync(initialObject);
        var updatedObject = new MapObject(5, new Coordinate(75, 75), 5, 5);
        ObjectUpdated? receivedEvent = null;
        _eventBroker.Subscribe<ObjectUpdated>(e => receivedEvent = e);

        // Act
        await _repository.UpdateAsync(updatedObject);
        var retrievedObject = await _repository.GetByIdAsync(updatedObject.Id);

        // Assert
        Assert.NotNull(retrievedObject);
        Assert.Equal(updatedObject.TopLeft, retrievedObject.TopLeft);
        Assert.NotNull(receivedEvent);
        Assert.Equal(updatedObject.Id, receivedEvent.EntityId);
        Assert.Equal(updatedObject.TopLeft, receivedEvent.ObjectCoordinates);
    }

    [Fact]
    public async Task RemoveAsync_Should_Remove_Object_And_Publish_Event()
    {
        // Arrange
        var testObjectId = 6u;
        var testObject = new MapObject(testObjectId, new Coordinate(100, 100), 5, 5);
        await _repository.AddAsync(testObject);
        ObjectRemoved? receivedEvent = null;
        _eventBroker.Subscribe<ObjectRemoved>(e => receivedEvent = e);

        // Act
        var isRemoved = await _repository.RemoveAsync(testObjectId);
        var afterRemove = await _repository.GetByIdAsync(testObjectId);

        // Assert
        Assert.True(isRemoved);
        Assert.Null(afterRemove);
        Assert.NotNull(receivedEvent);
        Assert.Equal(testObjectId, receivedEvent.EntityId);
    }
}