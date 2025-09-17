using System.Collections.Concurrent;
using MapService.Domain.ValueObjects;

namespace MapService.Domain.Abstractions;

public abstract class LayerEntity<TObject>
    where TObject : ObjectEntity
{
    public uint Width { get; protected set; }
    public uint Height { get;protected set; }
    public IList<TObject> Objects { get; protected set; }
}