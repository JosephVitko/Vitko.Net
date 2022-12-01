using System.Linq.Expressions;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace Vitko.Net.Azure.Blob;

public class BlobService<T> : ICrudService<T> where T : AbstractBlob
{
    private readonly BlobContainerClient _containerClient;
    
    public BlobService(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }
    
    public async Task<T?> GetItemAsync(string id)
    {
        var blobClient = _containerClient.GetBlobClient(id);
        if (!await blobClient.ExistsAsync())
            return null;
        
        // get file type
        var fileType = (await blobClient.GetPropertiesAsync()).Value.ContentType;
        
        // get file content
        await using var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream);
        
        // create object
        T item = (T) Activator.CreateInstance(typeof(T), id, stream, fileType)!;
        return item;
    }

    public async Task<IEnumerable<T?>> GetItemsAsync(string queryString)
    {
        throw new NotImplementedException("Searching cannot be done on blobs");
    }

    public async Task<IEnumerable<T?>> GetItemsAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException("Searching cannot be done on blobs");
    }

    public async Task<IEnumerable<T>?> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, bool @descending = false)
    {
        throw new NotImplementedException("Searching cannot be done on blobs");
    }

    public async Task<IEnumerable<T>?> GetItemsAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy, int skip, int take, bool @descending = false)
    {
        throw new NotImplementedException("Searching cannot be done on blobs");
    }

    /// <summary>
    /// Same as UpsertItemAsync
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<T> CreateItemAsync(T item)
    {
        return await UpsertItemAsync(item);
    }

    /// <summary>
    /// Allows appending to a blob
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<T> UpdateItemAsync(T item)
    {
        AppendBlobClient blobClient = _containerClient.GetAppendBlobClient(item.Id);
        if (!await blobClient.ExistsAsync())
            throw new Exception("Blob does not exist");
        
        await blobClient.AppendBlockAsync(item.DataStream);
        return item;
    }

    /// <summary>
    /// Allows overwriting a blob or creating a new one
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<T> UpsertItemAsync(T item)
    {
        var blobClient = _containerClient.GetBlobClient(item.Id);
        await blobClient.UploadAsync(item.DataStream, new BlobHttpHeaders
        {
            ContentType = item.ContentType
        });
        return item;
    }

    public async Task DeleteItemAsync(string id)
    {
        var blobClient = _containerClient.GetBlobClient(id);
        await blobClient.DeleteIfExistsAsync();
    }
}