namespace Vitko.Net.Azure.Blob;

/// <summary>
/// Represents any blob that can be uploaded to or downloaded from Azure Blob Storage.
/// </summary>
public class AbstractBlob
{
    public string Id { get; set; }
    public Stream DataStream { get; set; }
    public string ContentType { get; set; }
}