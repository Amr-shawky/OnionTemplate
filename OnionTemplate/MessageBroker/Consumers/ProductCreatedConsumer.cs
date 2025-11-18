using MediatR;
using OnionTemplate.MessageBroker.Messages;

namespace OnionTemplate.MessageBroker.Consumers
{
    public class ProductCreatedConsumer
    {
        IMediator _mediator;
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
