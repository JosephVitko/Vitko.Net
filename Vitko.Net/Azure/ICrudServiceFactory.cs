namespace Azure;

public interface ICrudServiceFactory
{
    // public void Initialize(IConfigurationSection configurationSection);

    public ICrudService<T> CreateService<T>(string? containerName) where T : class;
}