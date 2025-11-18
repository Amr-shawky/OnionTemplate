using MediatR;
using OnionTemplate.Application.DTOs.Order;

namespace OnionTemplate.CQRS.Orders.Orchestrators.AddOrderOrchestrator
{
    public record AddOrderOrchestrator(Guid userID, CreateOrderDto CreateOrderDto) : IRequest<Guid>;

    public class AddOrderOrchestratorHandler : IRequestHandler<AddOrderOrchestrator, Guid>
    {
        private readonly IMediator _mediator;
        public AddOrderOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<Guid> Handle(AddOrderOrchestrator request, CancellationToken cancellationToken)
        {

            var cartItems = await _mediator.Send(new Carts.Queries.GetCartItemsByUserQuery(request.userID));


            Guid userId = Guid.NewGuid(); // Replace with actual user ID retrieval logic.
            var orderId = await _mediator.Send(new Commands.AddOrderCommand(), cancellationToken);
            return orderId;
        }
    }
}
