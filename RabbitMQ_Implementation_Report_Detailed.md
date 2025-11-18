# RabbitMQ Implementation Report (Detailed & Corrected)

This report provides a detailed explanation of the RabbitMQ implementation in the OnionTemplate project, covering both the publisher and receiver components, with full, corrected code examples and an in-depth discussion of best practices.

## Publisher (Sender) Implementation

The publisher is responsible for sending messages to RabbitMQ. The implementation consists of a service that encapsulates the logic for creating exchanges, queues, and publishing messages, now updated with non-blocking initialization and proper resource management.

### 1. `IRabbitMQPublisherService` Interface

The application defines an interface `IRabbitMQPublisherService` which outlines the contract for the RabbitMQ publisher. This interface is located in `OnionTemplate/Application/Interfaces/Services/`.

**Full Code:**
```csharp
namespace OnionTemplate.Application.Interfaces.Services
{
    public interface IRabbitMQPublisherService
    {
        Task PublishMessage(string exchangeName, string routingKey, string message);
    }
}
```

### 2. `RabbitMQPublisherService` Class

The `RabbitMQPublisherService` class, located in `OnionTemplate/Infrastructure/Services/`, implements the `IRabbitMQPublisherService` interface. It has been refactored to handle asynchronous initialization and graceful disposal of resources.

**Full Code:**
```csharp
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
        await EnsureInitializedAsync();
        byte[] messageBody = System.Text.Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(exchange: exchangeName,
                                 routingKey: routingKey,
                                 body: messageBody);
    }


    public async Task CreateExchange(string exchangeName, string type = "direct")
    {
        await EnsureInitializedAsync();
        await _channel.ExchangeDeclareAsync(exchange: exchangeName, type: type);
    }

    public async Task CreateQueue(string queueName)
    {
        await EnsureInitializedAsync();
        await _channel.QueueDeclareAsync(queue: queueName,
                             durable: true,
                             autoDelete: false,
                             arguments: null);
    }

   public async Task BindQueue(string queueName, string exchangeName, string routingKey)
    {
        await EnsureInitializedAsync();
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
```

**Detailed Discussion:**

*   **Asynchronous Initialization:** The constructor no longer blocks with `.Result`. Instead, it uses a `Lazy<Task>` to manage the asynchronous initialization of the RabbitMQ connection and channel. The `EnsureInitializedAsync` method is called before any RabbitMQ operation to guarantee that the connection is established. This is a much safer and more efficient way to handle async initialization in a DI context.
*   **Resource Management:** The class now implements `IAsyncDisposable`. The `DisposeAsync` method ensures that the channel and connection are gracefully closed when the service is disposed, preventing resource leaks. This is crucial for the stability of the application.

### 3. Dependency Injection

The `RabbitMQPublisherService` is registered in `Program.cs` as a scoped service:

```csharp
builder.Services.AddScoped<IRabbitMQPublisherService, RabbitMQPublisherService>();
```

## Receiver (Consumer) Implementation

The receiver has been significantly improved to ensure message reliability and graceful shutdown.

### 1. Message Contracts
The message contracts remain the same.

**`BasicMessage.cs`**
```csharp
namespace OnionTemplate.MessageBroker.Messages
{
    public class BasicMessage
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
    }
}
```

**`ProductCreatedMessage.cs`**
```csharp
namespace OnionTemplate.MessageBroker.Messages
{
    public class ProductCreatedMessage : BasicMessage
    {
        public string Brand { get; set; }
        public string Name { get; set; }

        public double Price { get; set; }
    }
}
```

### 2. Consumer Classes

The consumer class now contains placeholder logic to demonstrate how it would interact with the application.

**`ProductCreatedConsumer.cs`**
```csharp
using MediatR;
using OnionTemplate.MessageBroker.Messages;
using System;

namespace OnionTemplate.MessageBroker.Consumers
{
    public class ProductCreatedConsumer
    {
        private readonly IMediator _mediator;
        public ProductCreatedConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Consume(BasicMessage basicMessage)
        {
            if (basicMessage is ProductCreatedMessage message)
            {
                // In a real application, you would use MediatR to send a command
                // or event to be handled by your application logic.
                // For example:
                // _mediator.Send(new CreateProductCommand(message.Name, message.Brand, message.Price));

                Console.WriteLine($"Consumed ProductCreatedMessage for product: {message.Name}");
            }
        }
    }
}
```

### 3. `RabbitMQConsumerService` Class

The `RabbitMQConsumerService` has been heavily refactored for correctness and robustness.

**Full Code:**
```csharp
using MediatR;
using OnionTemplate.MessageBroker.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
```

**Detailed Discussion:**

*   **Correct Message Acknowledgment:** The message acknowledgment logic is now correct. `BasicAck` is only called if the message is processed successfully. If an exception occurs, `BasicNack` is called to negatively acknowledge the message. The `requeue` parameter is set to `true`, which will send the message back to the queue to be processed again. For a more robust system, you would want to implement a dead-letter queue (DLQ) to handle messages that repeatedly fail.
*   **Graceful Shutdown:** The `StopAsync` method is now fully implemented. It checks if the channel and connection are open and closes them if they are. This is essential for a clean shutdown of the application.
*   **Configuration:** Hardcoded values like `"localhost"` and `"Bosta"` should be moved to `appsettings.json` in a production application to allow for easier configuration.

### 4. Dependency Injection

The `RabbitMQConsumerService` is registered in `Program.cs` as a hosted service:

```csharp
builder.Services.AddHostedService<RabbitMQConsumerService>();
```
This ensures that the consumer starts listening for messages as soon as the application starts.
