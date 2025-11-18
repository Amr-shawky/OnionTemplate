using OnionTemplate.Application.DTOs.Common;
using OnionTemplate.Application.DTOs.Product;

namespace OnionTemplate.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(Guid productId);
        Task<PaginatedResult<ProductDto>> GetProductsAsync(int page, int pageSize);
        Task<PaginatedResult<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, int page, int pageSize);
        Task<PaginatedResult<ProductDto>> SearchProductsAsync(string searchTerm, int page, int pageSize);
        Task<PaginatedResult<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, int page, int pageSize);
        Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int count);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(Guid productId, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(Guid productId);
        Task<bool> UpdateStockAsync(Guid productId, int quantity);
        Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeProductId = null);
    }
}

