using MediatR;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;

namespace OnionTemplate.CQRS.Products.Commands
{
    public record AddProductCommand(string Name, decimal Price) : IRequest<Guid>;

    public class AddProductCommandHandler: IRequestHandler<AddProductCommand, Guid>
    {
        private readonly IRepository<Product> _repository;
        private readonly IMediator _mediator;
        
        public AddProductCommandHandler(IRepository<Product> repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }
        public async Task<Guid> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            bool exists = await _mediator.Send(new Queries.IsProductExists(request.Name));

            if (exists)
            {
                throw new InvalidOperationException($"A product with the name '{request.Name}' already exists.");
            }

            var newProduct = new Core.Entities.Product
            {
                Name = request.Name,
                Price = request.Price,
                Description = string.Empty,
                SKU = string.Empty,
                StockQuantity = 0,
                IsActive = true
            };
            await _repository.AddAsync(newProduct);
            await _repository.SaveChangesAsync();

            return newProduct.Id;
        }
    }
}
