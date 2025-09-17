using System.Collections.Concurrent;
using MapService.Domain.ValueObjects;

namespace MapService.Domain.Abstractions;

public abstract class LayerEntity<TObject, TType>
    where TObject : ObjectEntity<TType>
{
    
}