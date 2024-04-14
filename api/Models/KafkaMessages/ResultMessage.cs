namespace Models.KafkaMessages;

public class ResultMessage
{
    public string Guid { get; set; }
    public string FileId { get; set; }
    public decimal Confidence { get; set; }
    public string Type { get; set; }
    public string Series { get; set; }
    public string Number { get; set; }
    public int? PageNumber { get; set; }
    public Dictionary<string,string> OptionalFields { get; set; }
}
