namespace Kafka.Interfaces;

public interface IKafkaProducesService
{
    Task WriteTraceLogAsync(object value);
    Task TestLog(object value);
}
