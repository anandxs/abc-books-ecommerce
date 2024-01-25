using AbcBooks.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AbcBooks.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        dbSet = _db.Set<T>();
    }

    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        dbSet.AddRange(entities);
    }

    public IEnumerable<T> GetAll()
    {
        return dbSet;
    }

    public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
    {
        return dbSet
            .Where(filter)
            .FirstOrDefault()!;
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        dbSet.RemoveRange(entities);
    }
}
