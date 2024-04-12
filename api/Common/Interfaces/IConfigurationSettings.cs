namespace Common.Interfaces;

public interface IConfigurationSettings
{
    public KafkaSettings KafkaSettings { get; }
}
