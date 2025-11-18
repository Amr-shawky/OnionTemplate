using MediatR;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;

namespace OnionTemplate.CQRS.Products.Queries
{
    public record IsProductExists(string name) : IRequest<bool>;

    public class IsProductExistsHandler : IRequestHandler<IsProductExists, bool>
    {
        private readonly IRepository<Product> _repository;

        public IsProductExistsHandler(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(IsProductExists request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetAllAsync();
            return products.Any(p => p.Name == request.name);
        }
    }
}
