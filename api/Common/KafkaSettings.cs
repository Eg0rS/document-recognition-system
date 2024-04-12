namespace Common;

public class KafkaSettings
{
    public string KafkaConnection { get; set; }
    public string KafkaTopicProducer { get; set; }
    public string KafkaTopicConsumer { get; set; }
    public string KafkaGroupIdConsumer { get; set; }
    public string KafkaGroupIdProducer { get; set; }
}

