using MediatR;

namespace OnionTemplate.CQRS.Products.Orchestrators
{
    public record DeleteProductOrchestrator(Guid id) : IRequest;

    public class DeleteProductOrchestratorHandler : IRequestHandler<DeleteProductOrchestrator>
    {
        private readonly IMediator _mediator;
        public DeleteProductOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<Unit> Handle(DeleteProductOrchestrator request, CancellationToken cancellationToken)
        {
            // Check Product is exist => Execute Query
            // Remove Product from all Carts => Execute Command
            // Remove Product from all Orders (set as inactive instead of delete) => Execute another Orchestrator
            // Delete Product Images from storage
            // Delete Product record from database

            // fire event

            await _mediator.Publish(new ProductDeletedEvent(request.id), cancellationToken);

            return Unit.Value;
        }
    }
}
