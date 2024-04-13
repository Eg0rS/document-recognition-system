using Common.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, string> consumer;
    private readonly ILogger<KafkaConsumerService> logger;
    private readonly KafkaEventHandler kafkaEventHandler;

    public KafkaConsumerService(IConfigurationSettings settings, ILogger<KafkaConsumerService> logger, KafkaEventHandler kafkaEventHandler)
    {
        this.logger = logger;
        this.kafkaEventHandler = kafkaEventHandler;
        var topic = settings.KafkaSettings.KafkaTopicConsumer;
        consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig
        {
            GroupId = settings.KafkaSettings.KafkaGroupIdConsumer,
            BootstrapServers = settings.KafkaSettings.KafkaConnection,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true,
            EnableAutoCommit = false,
        }).SetLogHandler((_, logMessage) =>
        {
            if (logMessage.Level != SyslogLevel.Info && logMessage.Level != SyslogLevel.Debug)
            {
                this.logger.LogError($"Kafka consumer error. {logMessage.Message}");
            }
        }).Build();
        consumer.Subscribe(topic);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);

            logger.LogInformation(consumeResult.Message.Key + " - " + consumeResult.Message.Value);
            await kafkaEventHandler.HandleAsync(consumeResult.Message.Value);

            consumer.Commit();
        }
    }

    public override void Dispose()
    {
        consumer.Dispose();
        base.Dispose();
    }
}
