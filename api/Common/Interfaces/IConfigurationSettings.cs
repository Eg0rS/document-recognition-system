namespace Common.Interfaces;

public interface IConfigurationSettings
{
    public KafkaSettings KafkaSettings { get; }
    public string DbConnection { get; }
}
