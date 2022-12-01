using Newtonsoft.Json;

namespace Vitko.Net.Azure.Cosmos;

/// <summary>
/// Represents a type of item that is stored in a CosmosDB container.
/// </summary>
public abstract class AbstractCosmosItem
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
}