using MapService.Domain.ValueObjects;

namespace MapService.Infrastructure.Messaging.Events;

public record ObjectUpdated(uint EntityId, Coordinate ObjectCoordinates)
{
    public uint EntityId { get; } = EntityId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Coordinate ObjectCoordinates { get; } =  ObjectCoordinates; 
}