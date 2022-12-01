namespace Azure.Blob;

public class AbstractBlob
{
    public string Id { get; set; }
    public Stream DataStream { get; set; }
    public string ContentType { get; set; }
}