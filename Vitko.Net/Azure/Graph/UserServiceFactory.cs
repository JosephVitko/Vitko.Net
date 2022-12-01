using Azure;
using Microsoft.Graph;

namespace Vitko.Net.Azure.Graph;

/// <summary>
/// Singleton class that stores configuration for initializing instances of <see cref="UserService{T}"/>.
/// </summary>
public class UserServiceFactory
{
    private static UserServiceFactory? _b2cUserServiceFactory;
    // possible to create instances for additional ADs, e.g. the admin AD
    
    private static readonly object _lock = new();
    
    private readonly string[] _scopes;
    private readonly string _clientId;
    private readonly string _tenantId;
    private readonly string _clientSecret;
    private readonly string[] _userFields;

    private UserServiceFactory(string[] scopes, string clientId, string tenantId, string clientSecret, string[] userFields)
    {
        _scopes = scopes;
        _clientId = clientId;
        _tenantId = tenantId;
        _clientSecret = clientSecret;
        _userFields = userFields;
    }

    public static UserServiceFactory B2CInstance
    {
        get
        {
            if (_b2cUserServiceFactory == null)
            {
                throw new Exception("B2CUserServiceFactory not initialized");
            }
            
            return _b2cUserServiceFactory;
        }
    }

    public static void InitializeB2C(string[] scopes, string clientId, string tenantId, string clientSecret, string[] userFields)
    {
        lock (_lock)
        {
            _b2cUserServiceFactory ??= new UserServiceFactory(
                scopes,
                clientId,
                tenantId,
                clientSecret,
                userFields
            );
        }
    }
    
    public ICrudService<T> CreateService<T>(string? containerName, Func<User, T?> convertUserToType, Func<T, User?> convertTypeToUser) where T : class
    {
        GraphService graphService = new(_scopes, _clientId, _tenantId, _clientSecret);
        return new UserService<T>(graphService, _userFields, convertUserToType, convertTypeToUser);
    }
}