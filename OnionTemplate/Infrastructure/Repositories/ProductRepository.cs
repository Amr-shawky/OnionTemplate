using Microsoft.EntityFrameworkCore;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;
using OnionTemplate.Infrastructure.Data;

namespace OnionTemplate.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive && 
                           (p.Name.Contains(searchTerm) || 
                            p.Description.Contains(searchTerm) ||
                            p.Brand!.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithImagesAsync(Guid productId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductImages.OrderBy(pi => pi.DisplayOrder))
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeProductId = null)
        {
            var query = _dbSet.Where(p => p.SKU == sku);
            
            if (excludeProductId.HasValue)
            {
                query = query.Where(p => p.Id != excludeProductId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task UpdateStockAsync(Guid productId, int quantity)
        {
            var product = await _dbSet.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity = quantity;
                _dbSet.Update(product);
            }
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive && p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }
    }
}

