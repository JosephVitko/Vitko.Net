using System.Linq.Expressions;
using Azure;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Vitko.Net.Azure.Cosmos;

public class CosmosDbService<T> : ICrudService<T> where T : class
{
    private readonly Container _container;
    
    public CosmosDbService(Container container)
    {
        // if (!typeof(T).IsAssignableFrom(typeof(AbstractCosmosItem)))
        // {
        //     throw new Exception("T must be a subclass of AbstractCosmosItem");
        // }
        
        _container = container;
    }
    
    public async Task<T?> GetItemAsync(string id)
    {
        try
        {
            ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
    
    public async Task<IEnumerable<T?>> GetItemsAsync(string queryString)
    {
        var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
        List<T> results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
    
        return results;
    }
    
    public async Task<IEnumerable<T?>> GetItemsAsync(Expression<Func<T, bool>> predicate)
    {
        var query = _container.GetItemLinqQueryable<T>(true)
            .Where(predicate)
            .ToFeedIterator();
        List<T> results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
    
        return results;
    }

    public async Task<IEnumerable<T>?> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool @descending = false)
    {
        var query = _container.GetItemLinqQueryable<T>(true)
            .Where(predicate)
            .OrderBy(orderBy)
            .ToFeedIterator();
        List<T> results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
    
        return results;
    }

    public async Task<IEnumerable<T>?> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, int skip, int take, bool @descending = false)
    {
        var query = _container.GetItemLinqQueryable<T>(true)
            .Where(predicate)
            .OrderBy(orderBy)
            .Skip(skip)
            .Take(take)
            .ToFeedIterator();
        List<T> results = new List<T>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
    
        return results;
    }

    public async Task<T> UpsertItemAsync(T item)
    {
        if (item is not AbstractCosmosItem cosmosItem)
        {
            throw new Exception("Item must be a subclass of AbstractCosmosItem");
        }
        
        ItemResponse<T> response = await _container.UpsertItemAsync<T>(item, new PartitionKey(cosmosItem.Id));
        return response.Resource;
    }
    
    public async Task<T> UpdateItemAsync(T item)
    {
        return await UpsertItemAsync(item);
    }
    
    public async Task<T> CreateItemAsync(T item)
    {
        if (item is not AbstractCosmosItem cosmosItem)
        {
            throw new Exception("Item must be a subclass of AbstractCosmosItem");
        }
        
        ItemResponse<T> response = await _container.CreateItemAsync<T>(item, new PartitionKey(cosmosItem.Id));
        return response.Resource;
    }

    public async Task DeleteItemAsync(string id)
    {
        await _container.DeleteItemAsync<T>(id, new PartitionKey(id));
    }
}