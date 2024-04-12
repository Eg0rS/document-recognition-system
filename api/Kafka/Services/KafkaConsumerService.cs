using Common.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConsumer<Null, string> consumer;
    private readonly ILogger<KafkaConsumerService> logger;

    public KafkaConsumerService(IConfigurationSettings settings, ILogger<KafkaConsumerService> logger)
    {
        this.logger = logger;
        var topic = settings.KafkaSettings.KafkaTopicConsumer;
        consumer = new ConsumerBuilder<Null, string>(new ConsumerConfig
        {
            GroupId = "result-consumer-group",
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
        var consumeResult = consumer.Consume();
        logger.LogInformation(consumeResult.Message.Key + " - " + consumeResult.Message.Value);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        var i = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);

            logger.LogInformation(consumeResult.Message.Key + " - " + consumeResult.Message.Value);
            
            consumer.Commit();
        }
    }

    public override void Dispose()
    {
        consumer.Dispose();
        base.Dispose();
    }
}
