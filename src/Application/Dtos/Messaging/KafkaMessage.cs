namespace IntegrationImport.Application.Dtos.Messaging;

public class KafkaMessage<TMessage> where TMessage : class
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public TMessage? Content { get; set; } = default;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}