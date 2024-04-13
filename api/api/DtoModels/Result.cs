using System.Text.Json.Serialization;

namespace api.DtoModels;

public class Result
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
    [JsonPropertyName("guid")]
    public string Guid { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("series")]
    public string Series { get; set; }
    [JsonPropertyName("number")]
    public string Number { get; set; }
    [JsonPropertyName("page_number")]
    public int? PageNumber { get; set; }
    [JsonPropertyName("confidence")]
    public decimal Confidence { get; set; }
    [JsonPropertyName("file_id")]
    public string FileId { get; set; }
    [JsonPropertyName("data")]
    public Dictionary<string,string> Data { get; set; }
    [JsonPropertyName("status")]
    public int Status { get; set; }
}
