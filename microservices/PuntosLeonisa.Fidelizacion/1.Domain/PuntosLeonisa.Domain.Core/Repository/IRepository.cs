

using System.Linq.Expressions;

public interface IRepository<T> where T: class
{
    Task<IEnumerable<T>> GetAll();
    Task<T> GetById(string id);
    Task Add(T entity);
    Task AddRange(T[] entities);
    Task Update(T entity);
    Task Delete(T entity);
    Task<T> GetSequence(T entity);

    // Get By Predicate 
    Task<IEnumerable<T>> GetByPredicateAsync(Expression<Func<T, bool>> predicate);
}

