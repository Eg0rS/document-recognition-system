using Common;
using Common.Interfaces;
using Confluent.Kafka;
using Kafka.Interfaces;
using Microsoft.Extensions.Logging;
using Models.KafkaMessages;

namespace Kafka.Services;

public class KafkaProducesService : IKafkaProducesService
{
    private readonly IProducer<Null, RequestMessage> producer;
    private readonly ILogger<KafkaProducesService> logger;
    private readonly string topic;

    public KafkaProducesService(IConfigurationSettings settings, ILogger<KafkaProducesService> logger)
    {
        this.logger = logger;
        topic = settings.KafkaSettings.KafkaTopicProducer;
        producer = new ProducerBuilder<Null, RequestMessage>(new ProducerConfig
        {
            LingerMs = 2000, BatchSize = 1000, QueueBufferingMaxMessages = 500, BootstrapServers = settings.KafkaSettings.KafkaConnection
        }).SetLogHandler((_, logMessage) =>
        {
            if (logMessage.Level != SyslogLevel.Info && logMessage.Level != SyslogLevel.Debug)
            {
                this.logger.LogError($"Kafka producer error. {logMessage.Message}");
            }
        }).SetValueSerializer(new RequestMessageSerializer()).Build();
    }

    public async Task WriteTraceLogAsync(RequestMessage value)
    {
        try
        {
            await producer.ProduceAsync(topic, new Message<Null, RequestMessage> { Key = null, Value = value });
        }
        catch (ProduceException<Ignore, string> exc)
        {
            logger.LogError("Send message to kafka failed: {ExcError}. Message: {Json}", exc.Error, value.ToJson());
            _ = Task.CompletedTask;
        }
    }

    public async Task TestLog(RequestMessage value)
    {
        try
        {
            await producer.ProduceAsync("test-topic", new Message<Null, RequestMessage> { Value = value });
        }
        catch (ProduceException<Null, string> exc)
        {
            logger.LogError("Send message to kafka failed: {ExcError}. Message: {Json}", exc.Error, value.ToJson());
            _ = Task.CompletedTask;
        }
    }
}
