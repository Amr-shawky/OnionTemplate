using OnionTemplate.Application.Interfaces.Services;
using RabbitMQ.Client;

namespace OnionTemplate.Infrastructure.Services;

public class RabbitMQPublisherService : IRabbitMQPublisherService, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    // Use a Lazy<T> to ensure the initialization is thread-safe and happens only once.
    private readonly Lazy<Task> _initializationTask;

    public RabbitMQPublisherService()
    {
        _initializationTask = new Lazy<Task>(InitializeRabbitMQAsync);
    }

    private async Task InitializeRabbitMQAsync()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    private async Task EnsureInitializedAsync()
    {
        await _initializationTask.Value;
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

    public async ValueTask DisposeAsync()
    {
        if (_channel?.IsOpen ?? false)
        {
            await _channel.CloseAsync();
        }
        if (_connection?.IsOpen ?? false)
        {
            await _connection.CloseAsync();
        }
    }
}
