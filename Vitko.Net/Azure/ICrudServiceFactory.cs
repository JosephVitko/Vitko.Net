namespace Vitko.Net.Azure;

/// <summary>
/// Stores any configuration settings for initializing an ICrudService and contains a method for creating an instance of the service.
/// </summary>
public interface ICrudServiceFactory
{
    // public void Initialize(IConfigurationSection configurationSection);

    public ICrudService<T> CreateService<T>(string? containerName) where T : class;
}