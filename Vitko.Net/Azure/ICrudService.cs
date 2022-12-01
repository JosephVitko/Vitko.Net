using System.Linq.Expressions;

namespace Vitko.Net.Azure;

public interface ICrudService<T> where T : class
{
    public Task<T?> GetItemAsync(string id);
    
    public Task<IEnumerable<T?>> GetItemsAsync(string queryString);
    
    public Task<IEnumerable<T?>> GetItemsAsync(Expression<Func<T, bool>> predicate);
    
    public Task<IEnumerable<T>?> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool descending = false);
    
    public Task<IEnumerable<T>?> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, int skip, int take, bool descending = false);

    public Task<T> CreateItemAsync(T item);
    
    public Task<T> UpdateItemAsync(T item);

    public Task<T> UpsertItemAsync(T item);
    
    public Task DeleteItemAsync(string id);

}