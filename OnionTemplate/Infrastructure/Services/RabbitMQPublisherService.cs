using OnionTemplate.Application.Interfaces.Services;
using RabbitMQ.Client;

namespace OnionTemplate.Infrastructure.Services;

public class RabbitMQPublisherService : IRabbitMQPublisherService
{
    IConnection _connection;
    IChannel _channel;

    public RabbitMQPublisherService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnectionAsync().Result;

        _channel = _connection.CreateChannelAsync().Result;
    }

    public async Task PublishMessage(string exchangeName, string routingKey, string message)
    {
        byte[] messageBody = System.Text.Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(exchange: exchangeName,
                                 routingKey: routingKey,
                                 body: messageBody);
    }


    public async Task CreateExchange(string exchangeName, string type = "direct")
    {
        await _channel.ExchangeDeclareAsync(exchange: exchangeName, type: type);
    }

    public async Task CreateQueue(string queueName)
    {
        await _channel.QueueDeclareAsync(queue: queueName,
                             durable: true,
                             autoDelete: false,
                             arguments: null);
    }

   public async Task BindQueue(string queueName, string exchangeName, string routingKey)
    {
        await _channel.QueueBindAsync(queue: queueName,
                           exchange: exchangeName,
                           routingKey: routingKey);
    }
}
