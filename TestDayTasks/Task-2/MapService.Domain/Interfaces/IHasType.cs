namespace MapService.Domain.Interfaces;

public interface IHasType<T>
{
    public T Type { get; set; }
}