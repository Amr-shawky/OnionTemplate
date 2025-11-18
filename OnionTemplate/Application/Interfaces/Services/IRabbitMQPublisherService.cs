namespace OnionTemplate.Application.Interfaces.Services
{
    public interface IRabbitMQPublisherService
    {
        Task PublishMessage(string exchangeName, string routingKey, string message);
    }
}
