using Database;
using Database.Interfaces;
using Models.KafkaMessages;
using Newtonsoft.Json;

namespace Kafka.Services;

public class KafkaEventHandler
{
    private readonly IConnection connection;

    public KafkaEventHandler(IConnection connection)
    {
        this.connection = connection;
    }

    public event EventHandler<ResultMessage> MessageReceived;

    public async Task HandleAsync(string message)
    {
        var massageJson = JsonConvert.DeserializeObject<ResultMessage>(message);
        OnMessageReceived(massageJson);
        var queryObject = new QueryObject(
            "INSERT INTO results (guid, type, series, number, confidence, page_number, file_id, data) VALUES (@guid, @type, @series, @number, @confidence, @page_number, @file_id, @data)",
        new
        {
            guid = massageJson.Guid,
            type = massageJson.Type,
            series = massageJson.Series,
            number = massageJson.Number,
            confidence = massageJson.Confidence,
            page_number = massageJson.PageNumber,
            file_id = massageJson.FileId,
            data = JsonConvert.SerializeObject(massageJson.Data)
        });
    }

    protected virtual void OnMessageReceived(ResultMessage e)
    {
        MessageReceived?.Invoke(this, e);
    }
}
