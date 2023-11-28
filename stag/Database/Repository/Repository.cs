using Microsoft.EntityFrameworkCore;

namespace stag.Database.Repository;

public abstract class Repository<T>
{
    public Repository()
    {
        
    }

    public void Create(T item)
    {
        
    }

    public IEnumerable<T> Get()
    {
        return null;
    }

    public void Patch(T item)
    {
        
    }
}