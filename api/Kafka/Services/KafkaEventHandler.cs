namespace Kafka.Services;

public class KafkaEventHandler
{
    public event EventHandler<string> MessageReceived;
    public async Task HandleAsync(string message)
    {
        OnMessageReceived(message);
    }

    protected virtual void OnMessageReceived(string e)
    {
        MessageReceived?.Invoke(this, e);
    }
}
