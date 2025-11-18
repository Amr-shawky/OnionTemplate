using AutoMapper;
using MediatR;
using OnionTemplate.Application.DTOs.Product;
using OnionTemplate.Core.Entities;
using OnionTemplate.Application.Interfaces;

namespace OnionTemplate.CQRS.Products.Queries
{
    public record GetProductByIDQuery(Guid ProductID) : IRequest<ProductDto>;

    public class GetProductByIDQueryHandler : IRequestHandler<GetProductByIDQuery, ProductDto>
    {
        private readonly IRepository<Product> _repository;
        //IMapper _mapper;

        public GetProductByIDQueryHandler(IRepository<Product> repository)
        { 
            _repository = repository;
        }

        public async Task<ProductDto> Handle(GetProductByIDQuery request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.ProductID);
            
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductID} not found.");
            }

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
            };
        }
    }

}
