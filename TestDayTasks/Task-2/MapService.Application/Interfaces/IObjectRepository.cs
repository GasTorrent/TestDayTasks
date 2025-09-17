using MapService.Domain.ValueObjects;

namespace MapService.Application.Interfaces;

public interface IObjectRepository<TObject>
{
    Task AddAsync(TObject obj);
    Task UpdateAsync(TObject obj);
    Task<bool> RemoveAsync(uint id);
    Task<TObject?> GetByIdAsync(uint id);
    Task<TObject?> GetByCoordinateAsync(Coordinate coord);
    Task<IEnumerable<TObject>> GetInAreaAsync(Area area);
}
