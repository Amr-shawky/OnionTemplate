using Microsoft.EntityFrameworkCore;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;
using OnionTemplate.Infrastructure.Data;

namespace OnionTemplate.Infrastructure.Repositories
{
    public class CartRepository : Repository<CartItem>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Include(ci => ci.Product)
                .ThenInclude(p => p.ProductImages)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();
        }

        public async Task<CartItem?> GetCartItemAsync(Guid userId, Guid productId)
        {
            return await _dbSet
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cartItems = await _dbSet.Where(ci => ci.UserId == userId).ToListAsync();
            _dbSet.RemoveRange(cartItems);
        }

        public async Task<int> GetCartItemCountAsync(Guid userId)
        {
            return await _dbSet
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.Quantity);
        }

        public async Task<decimal> GetCartTotalAsync(Guid userId)
        {
            return await _dbSet
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.Quantity * ci.Product.Price);
        }
    }
}

