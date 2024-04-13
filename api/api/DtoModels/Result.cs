using System.Text.Json.Serialization;

namespace api.DtoModels;

public class Result
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
    [JsonPropertyName("guid")]
    public string Guid { get; set; }
    [JsonPropertyName("file_id")]
    public string FileId { get; set; }
    [JsonPropertyName("data")]
    public Dictionary<string,string> Data { get; set; }
}
