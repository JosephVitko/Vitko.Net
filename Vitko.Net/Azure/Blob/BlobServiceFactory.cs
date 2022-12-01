using Azure;
using Azure.Storage.Blobs;

namespace Vitko.Net.Azure.Blob;

public class BlobServiceFactory
{
    private static BlobServiceFactory? _instance;
    private static readonly object _lock = new();

    private BlobServiceClient _blobServiceClient;

    private BlobServiceFactory(string connectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public static BlobServiceFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new Exception("BlobServiceFactory not initialized");
            }
            
            return _instance;
        }
    }

    public static void Initialize(string connectionString)
    {
        lock (_lock)
        {
            _instance ??= new BlobServiceFactory(connectionString);
        }
    }
    
    public ICrudService<T> CreateService<T>(string containerName) where T : AbstractBlob
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return new BlobService<T>(containerClient);
    }

}