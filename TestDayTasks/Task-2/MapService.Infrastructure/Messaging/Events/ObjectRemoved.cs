using MapService.Domain.ValueObjects;

namespace MapService.Infrastructure.Messaging.Events;

public record ObjectRemoved(uint EntityId)
{
    public uint EntityId { get; } = EntityId;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}