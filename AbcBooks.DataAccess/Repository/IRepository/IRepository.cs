using System.Linq.Expressions;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface IRepository<T> where T : class
{
    T GetFirstOrDefault(Expression<Func<T, bool>> filter);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
