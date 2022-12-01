using Microsoft.Azure.Cosmos;

namespace Azure.Cosmos;

/// <summary>
/// Singleton class used to generate cosmos db service instances for each container
/// </summary>
public class CosmosDbServiceFactory : ICrudServiceFactory
{
    private static CosmosDbServiceFactory? _instance;
    private static readonly object _lock = new();

    private CosmosClient _cosmosClient;
    private Database _database;
    
    private CosmosDbServiceFactory(string connectionString, string databaseName)
    {
        _cosmosClient = new CosmosClient(connectionString);
        _database = _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName).Result.Database;
    }

    public static CosmosDbServiceFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new Exception("CosmosDbServiceFactory has not been initialized");
            }

            return _instance;
        }
    }
    
    public static void Initialize(string connectionString, string databaseName)
    {
        lock (_lock)
        {
            _instance ??= new CosmosDbServiceFactory(
                connectionString,
                databaseName
            );
        }
    }
    
    public ICrudService<T> CreateService<T>(string? containerName) where T : class
    {
        if (containerName == null)
        {
            containerName = typeof(T).Name;
        }
        
        var container = _database.CreateContainerIfNotExistsAsync(containerName, "/id").Result;
        var service = new CosmosDbService<T>(container);
        return service;
    }

}