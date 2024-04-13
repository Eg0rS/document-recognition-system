using System.Text.Json.Serialization;

namespace api.DtoModels;

public class ResultForHackTask
{
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
}
