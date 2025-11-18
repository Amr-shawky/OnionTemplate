using OnionTemplate.Core.Entities;

namespace OnionTemplate.Application.Interfaces
{
    public interface ICartRepository : IRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetCartItemsByUserAsync(Guid userId);
        Task<CartItem?> GetCartItemAsync(Guid userId, Guid productId);
        Task ClearCartAsync(Guid userId);
        Task<int> GetCartItemCountAsync(Guid userId);
        Task<decimal> GetCartTotalAsync(Guid userId);
    }
}

