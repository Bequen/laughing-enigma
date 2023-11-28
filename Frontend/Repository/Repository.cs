namespace Frontend.Repository;

public abstract class Repository<T>
{
    public abstract IEnumerable<T> Get();
}