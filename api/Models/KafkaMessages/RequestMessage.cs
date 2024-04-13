using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Models.KafkaMessages;

public class RequestMessage
{
    public string Guid { get; set; }
    public string FileId { get; set; }
}

public class RequestMessageSerializer : ISerializer<RequestMessage>
{
    public byte[] Serialize(RequestMessage data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
    }
}
