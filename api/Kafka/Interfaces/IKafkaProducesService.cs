using Models.KafkaMessages;

namespace Kafka.Interfaces;

public interface IKafkaProducesService
{
    Task WriteTraceLogAsync(RequestMessage value);
    Task TestLog(RequestMessage value);
}
