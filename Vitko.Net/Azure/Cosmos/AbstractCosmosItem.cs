using Newtonsoft.Json;

namespace Azure.Cosmos;

public abstract class AbstractCosmosItem
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
}