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
            var message = basicMessage as ProductCreatedMessage;

        }
    }
}
