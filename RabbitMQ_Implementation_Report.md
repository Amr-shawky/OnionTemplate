# RabbitMQ Implementation Report

This report provides a detailed explanation of the RabbitMQ implementation in the OnionTemplate project, covering both the publisher and receiver components.

## Publisher (Sender) Implementation

The publisher is responsible for sending messages to RabbitMQ. The implementation consists of a service that encapsulates the logic for creating exchanges, queues, and publishing messages.

### 1. `IRabbitMQPublisherService` Interface

The application defines an interface `IRabbitMQPublisherService` which outlines the contract for the RabbitMQ publisher. This interface is located in `OnionTemplate/Application/Interfaces/Services/`.

### 2. `RabbitMQPublisherService` Class

The `RabbitMQPublisherService` class, located in `OnionTemplate/Infrastructure/Services/`, implements the `IRabbitMQPublisherService` interface.

**Key Responsibilities:**

*   **Connection and Channel Management:**
    *   Establishes a connection to the RabbitMQ server in the constructor using `ConnectionFactory`. The hostname is currently hardcoded to `"localhost"`.
    *   Creates a channel from the connection, which is used for all subsequent operations.

*   **Exchange Creation:**
    *   The `CreateExchange` method allows for the creation of an exchange with a specified name and type (defaulting to "direct"). This is a fundamental step to ensure that messages can be routed correctly.

*   **Queue Creation:**
    *   The `CreateQueue` method creates a durable, non-auto-delete queue. This ensures that the queue and its messages will survive a broker restart.

*   **Queue Binding:**
    *   The `BindQueue` method binds a queue to an exchange using a routing key. This is how the exchange knows which queue(s) to route messages to.

*   **Message Publishing:**
    *   The `PublishMessage` method takes an exchange name, a routing key, and a message string as input.
    *   It converts the message string to a byte array using `UTF-8` encoding.
    *   It then uses `_channel.BasicPublishAsync` to publish the message to the specified exchange with the given routing key.

### 3. Dependency Injection

The `RabbitMQPublisherService` is registered in `Program.cs` as a scoped service:

```csharp
builder.Services.AddScoped<IRabbitMQPublisherService, RabbitMQPublisherService>();
```

This makes it available for injection into other services, such as application services or controllers, that need to publish messages.

## Receiver (Consumer) Implementation

The receiver is implemented as a background service that continuously listens for and processes messages from a RabbitMQ queue.

### 1. `RabbitMQConsumerService` Class

The `RabbitMQConsumerService` class is a hosted service (`IHostedService`) that runs in the background for the lifetime of the application.

**Key Responsibilities:**

*   **Connection and Channel Management:**
    *   Similar to the publisher, it establishes a connection and creates a channel to the RabbitMQ server in the constructor.

*   **Consumer Setup:**
    *   In the `StartAsync` method, it creates an `AsyncEventingBasicConsumer`.
    *   It subscribes to the `ReceivedAsync` event of the consumer, which is triggered when a message is received. This is a "push" mechanism, where the server pushes messages to the consumer.
    *   It starts consuming messages from a queue named `"Bosta"` using `_channel.BasicConsumeAsync`.

*   **Message Handling (`Consumer_ReceivedAync`):**
    *   This method is the core of the message processing logic.
    *   It retrieves the message body as a byte array and converts it to a string.
    *   It deserializes the JSON message into a `BasicMessage` object to determine the message `Type`.
    *   It then uses the message `Type` to dynamically determine the full type of the message and deserializes the message again into the specific message type.

*   **Dynamic Consumer Invocation (`InvokeConsumer`):**
    *   This method uses reflection to dynamically instantiate and invoke the correct consumer class based on the message type.
    *   For example, if the message type is `"ProductCreatedMessage"`, it will look for a class named `"ProductCreatedConsumer"`.
    *   It creates an instance of the consumer, passing in the `IMediator` instance.
    *   It then invokes the `Consume` method on the consumer instance, passing the deserialized message.

*   **Message Acknowledgment:**
    *   Crucially, it uses `_channel.BasicAckAsync` to acknowledge the message after it has been processed. This tells RabbitMQ that the message has been successfully handled and can be safely removed from the queue. This is done in a `finally` block to ensure that the message is acknowledged even if an error occurs during processing, preventing message re-delivery for failed processing attempts.

### 2. Message Contracts

The `OnionTemplate/MessageBroker/Messages/` directory contains the message contracts:
*   `BasicMessage.cs`: A base class for all messages, containing common properties like `Date` and `Type`.
*   `ProductCreatedMessage.cs`: A specific message type that inherits from `BasicMessage` and includes product-related data.

### 3. Consumer Classes

The `OnionTemplate/MessageBroker/Consumers/` directory contains the consumer classes.
*   `ProductCreatedConsumer.cs`: This class is responsible for handling `ProductCreatedMessage`s. It receives the message and can use other services (like `IMediator`) to perform business logic.

### 4. Dependency Injection

The `RabbitMQConsumerService` is registered in `Program.cs` as a hosted service:

```csharp
builder.Services.AddHostedService<RabbitMQConsumerService>();
```

This ensures that the consumer starts listening for messages when the application starts.
