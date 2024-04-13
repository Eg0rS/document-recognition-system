namespace api.DbModels;

public class DbResult
{
    public int Id { get; set; }
    public string Guid { get; set; }
    public string FileId { get; set; }
    public string Data { get; set; }
    public string Type { get; set; }
    public string Series { get; set; }
    public string Number { get; set; }
    public int? PageNumber { get; set; }
    public decimal Confidence { get; set; }
}
