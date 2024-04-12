using Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Common;

public class ConfigurationSettings : IConfigurationSettings
{
    private readonly IConfiguration configuration;

    public ConfigurationSettings(IConfiguration configuration)

    {
        this.configuration = configuration;
    }

    public KafkaSettings KafkaSettings => new KafkaSettings
    {
        KafkaConnection = configuration.GetSection("Kafka").GetSection("Connection").Value,
        KafkaTopicProducer = configuration.GetSection("Kafka").GetSection("TopicProducer").Value,
        KafkaTopicConsumer = configuration.GetSection("Kafka").GetSection("TopicConsumer").Value,
        KafkaGroupIdConsumer = configuration.GetSection("Kafka").GetSection("GroupIdConsumer").Value,
        KafkaGroupIdProducer = configuration.GetSection("Kafka").GetSection("GroupIdProducer").Value
    };
}
