using Microsoft.EntityFrameworkCore;
using OnionTemplate.Core.Entities;

namespace OnionTemplate.Application.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<Product?> GetProductWithImagesAsync(Guid productId);
        Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeProductId = null);
        Task UpdateStockAsync(Guid productId, int quantity);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}
