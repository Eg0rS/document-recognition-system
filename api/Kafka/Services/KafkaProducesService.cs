using Common;
using Common.Interfaces;
using Confluent.Kafka;
using Kafka.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kafka.Services;

public class KafkaProducesService : IKafkaProducesService
{
    private readonly IProducer<Null, string> producer;
    private readonly ILogger<KafkaProducesService> logger;
    private readonly string topic;

    public KafkaProducesService(IConfigurationSettings settings, ILogger<KafkaProducesService> logger)
    {
        this.logger = logger;
        topic = settings.KafkaSettings.KafkaTopicProducer;
        producer = new ProducerBuilder<Null, string>(new ProducerConfig
        {
            LingerMs = 2000,
            BatchSize = 1000,
            QueueBufferingMaxMessages = 500,
            BootstrapServers = settings.KafkaSettings.KafkaConnection
        }).SetLogHandler((_, logMessage) =>
        {
            if (logMessage.Level != SyslogLevel.Info && logMessage.Level != SyslogLevel.Debug)
            {
                this.logger.LogError($"Kafka producer error. {logMessage.Message}");
            }
        }).Build();
    }

    public async Task WriteTraceLogAsync(object value)
    {
        try
        {
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = value.ToJson() });
        }
        catch (ProduceException<Null, string> exc)
        {
            logger.LogError($"Send message to kafka failed: {exc.Error}. Message: {value.ToJson()}");
            _ = Task.CompletedTask;
        }
    }
    public async Task TestLog(object value)
    {
        try
        {
            await producer.ProduceAsync("test-topic", new Message<Null, string> { Value = value.ToJson() });
        }
        catch (ProduceException<Null, string> exc)
        {
            logger.LogError($"Send message to kafka failed: {exc.Error}. Message: {value.ToJson()}");
            _ = Task.CompletedTask;
        }
    }
}
