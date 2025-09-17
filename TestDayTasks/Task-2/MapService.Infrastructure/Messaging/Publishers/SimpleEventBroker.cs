using MapService.Application.Interfaces;

namespace MapService.Infrastructure.Messaging.Publishers;

public class SimpleEventBroker : IEventPublisher
{
    private readonly Dictionary<Type, List<object>> _handlers = new();

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        var eventType = typeof(TEvent);
        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = [];
        }
        _handlers[eventType].Add(handler);
    }

    public void Publish<TEvent>(TEvent @event)
    {
        var eventType = typeof(TEvent);
        if (!_handlers.TryGetValue(eventType, out var handlers)) return;
        foreach (var handler in handlers)
        {
            if (handler is Action<TEvent> typedHandler)
            {
                typedHandler(@event);
            }
        }
    }
}