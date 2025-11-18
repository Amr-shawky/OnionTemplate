using MediatR;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;

namespace OnionTemplate.CQRS.Orders.Commands
{
    public record AddOrderCommand() : IRequest<Guid>;

    public class AddOrderCommandHandler : IRequestHandler<AddOrderCommand, Guid>
    {
        IRepository<Order> _repository;

        public AddOrderCommandHandler(IRepository<Order> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(AddOrderCommand request, CancellationToken cancellationToken)
        {
            // Implement the logic to add a new order here.
            // For now, we'll just return a new GUID as a placeholder.
            return await Task.FromResult(Guid.NewGuid());
        }
    }

}
