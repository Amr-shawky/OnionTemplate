using OnionTemplate.Application.DTOs.Cart;

namespace OnionTemplate.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(Guid userId);
        Task<CartItemDto> AddToCartAsync(Guid userId, AddToCartDto addToCartDto);
        Task<CartItemDto> UpdateCartItemAsync(Guid userId, Guid productId, UpdateCartItemDto updateCartItemDto);
        Task<bool> RemoveFromCartAsync(Guid userId, Guid productId);
        Task<bool> ClearCartAsync(Guid userId);
        Task<int> GetCartItemCountAsync(Guid userId);
        Task<decimal> GetCartTotalAsync(Guid userId);
    }
}

