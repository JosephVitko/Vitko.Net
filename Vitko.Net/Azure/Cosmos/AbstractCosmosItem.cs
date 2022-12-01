using Newtonsoft.Json;

namespace Vitko.Net.Azure.Cosmos;

public abstract class AbstractCosmosItem
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
}