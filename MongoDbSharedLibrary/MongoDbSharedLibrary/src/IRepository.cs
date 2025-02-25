using System.Linq.Expressions;

namespace MongoDbSharedLibrary;

public interface IRepository<T> where T : IEntity
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task<T> GetByIdAsync(string id);
    Task<T> GetByIdAsync(Expression<Func<T, bool>> filter);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string id);
}