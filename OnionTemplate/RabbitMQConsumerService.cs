
using MediatR;
using OnionTemplate.MessageBroker.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OnionTemplate
{
    public class RabbitMQConsumerService : IHostedService
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly IMediator _mediator;

        public RabbitMQConsumerService(IMediator mediator)
        {
            _mediator = mediator;
        }

        private async Task InitializeRabbitMQAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeRabbitMQAsync(cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += Consumer_ReceivedAync;

            await _channel.BasicConsumeAsync("Bosta", autoAck: false, consumer: consumer);
        }

        private async Task Consumer_ReceivedAync(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var message = Encoding.UTF8.GetString(@event.Body.ToArray());
                var basicMessage = GetMessage(message);
                InvokeConsumer(basicMessage);

                // Acknowledge the message after successful processing
                await _channel.BasicAckAsync(@event.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using Serilog or ILogger)
                Console.WriteLine($"Error processing message: {ex.Message}");

                // Negatively acknowledge the message and requeue it for another attempt.
                // In a real-world scenario, you might want to implement a retry mechanism
                // with a dead-letter queue after a certain number of retries.
                await _channel.BasicNackAsync(@event.DeliveryTag, multiple: false, requeue: true);
            }
        }

        private void InvokeConsumer(BasicMessage basicMessage)
        {
            var namespaceName = "OnionTemplate.MessageBroker.Consumers";
            var typeName = basicMessage.Type.Replace("Message", "Consumer");

            // Ensure the assembly name is correct for type resolution.
            Type type = Type.GetType($"{namespaceName}.{typeName}, OnionTemplate");

            if (type == null)
            {
                throw new InvalidOperationException($"Consumer type not found for message type: {basicMessage.Type}");
            }

            var consumer = Activator.CreateInstance(type, _mediator);
            var methodInfo = type.GetMethod("Consume");

            methodInfo?.Invoke(consumer, new object[] { basicMessage });
        }

        private BasicMessage GetMessage(string message)
        {
            var basicMessage = System.Text.Json.JsonSerializer.Deserialize<BasicMessage>(message);
            if (basicMessage?.Type == null)
            {
                throw new InvalidOperationException("Message type is missing.");
            }

            var @namespace = "OnionTemplate.MessageBroker.Messages";
            // Ensure the assembly name is correct for type resolution.
            Type type = Type.GetType($"{@namespace}.{basicMessage.Type}, OnionTemplate");

            if (type == null)
            {
                throw new InvalidOperationException($"Message contract type not found: {basicMessage.Type}");
            }

            return System.Text.Json.JsonSerializer.Deserialize(message, type) as BasicMessage ?? throw new InvalidOperationException("Failed to deserialize message.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel?.IsOpen ?? false)
            {
                _channel.Close();
            }
            if (_connection?.IsOpen ?? false)
            {
                _connection.Close();
            }
            return Task.CompletedTask;
        }
    }
}
