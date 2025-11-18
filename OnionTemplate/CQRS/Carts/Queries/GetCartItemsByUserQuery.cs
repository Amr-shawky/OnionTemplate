using MediatR;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;

namespace OnionTemplate.CQRS.Carts.Queries
{
    public record GetCartItemsByUserQuery(Guid userID) : IRequest<IEnumerable<CartItem>>;

    public class GetCartItemsByUserQueryHandler : IRequestHandler<GetCartItemsByUserQuery, IEnumerable<CartItem>>
    {
        private readonly IRepository<CartItem> _repository;
        public GetCartItemsByUserQueryHandler(IRepository<CartItem> repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<CartItem>> Handle(GetCartItemsByUserQuery request, CancellationToken cancellationToken)
        {
            var cartItems = await _repository.GetAllAsync();
            return cartItems.Where(ci => ci.UserId == request.userID).ToList();
        }
    }
}
