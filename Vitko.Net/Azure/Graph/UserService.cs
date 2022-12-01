using System.Linq.Expressions;
using Azure;
using Microsoft.Graph;

namespace Vitko.Net.Azure.Graph;

public class UserService<T> : ICrudService<T> where T : class
{
    private readonly GraphService _graphService;
    private readonly string[] _userFields;
    
    // function to convert a user to a T
    private readonly Func<User, T?> _convertUserToType;
    
    // function to convert a T to a user
    private readonly Func<T, User?> _convertTypeToUser;
    
    
    public UserService(GraphService graphService, string[] userFields, Func<User, T?> convertUserToType, Func<T, User?> convertTypeToUser)
    {
        // if (!typeof(T).IsAssignableFrom(typeof(AbstractUser)))
        // {
        //     throw new ArgumentException("T must be a subclass of AbstractUser");
        // }
        
        _graphService = graphService;
        _userFields = userFields;
        _convertUserToType = convertUserToType;
        _convertTypeToUser = convertTypeToUser;
    }

    public async Task<T?> GetItemAsync(string id)
    {
        User? user = await _graphService.GetUser(id, new List<string>(_userFields));
        return user == null ? null : _convertUserToType(user);
    }

    public async Task<IEnumerable<T?>> GetItemsAsync(string queryString)
    {
        var users = await _graphService.GetUsersByQuery(queryString, new List<string>(_userFields));
        
        return users.Select(_convertUserToType);
    }

    public async Task<IEnumerable<T?>> GetItemsAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException("Getting users with a linq expression is not yet supported. Please use a query string instead.");
    }

    public async Task<IEnumerable<T>?> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool descending = false)
    {
        throw new NotImplementedException("Getting users with a linq expression is not yet supported. Please use a query string instead.");
    }

    public async Task<IEnumerable<T>?> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, int skip, int take, bool descending = false)
    {
        throw new NotImplementedException("Getting users with a linq expression is not yet supported. Please use a query string instead.");
    }

    public async Task<T> CreateItemAsync(T item)
    {
        var user = _convertTypeToUser(item);
        
        await _graphService.CreateUser(user);
        return _convertUserToType(user)!;
    }

    public async Task<T> UpdateItemAsync(T item)
    {
        var user = _convertTypeToUser(item);
        
        await _graphService.UpdateUser(user);
        return _convertUserToType(user)!;
    }

    public async Task<T> UpsertItemAsync(T item)
    {
        // TODO: Implement this, for now just call update
        return await UpdateItemAsync(item);
    }

    public async Task DeleteItemAsync(string id)
    {
        await _graphService.DeleteUser(id);
    }
}