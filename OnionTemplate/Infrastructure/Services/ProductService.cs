using OnionTemplate.Application.DTOs.Common;
using OnionTemplate.Application.DTOs.Product;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Application.Interfaces.Services;

namespace OnionTemplate.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        IProductRepository _productRepository;
       // ICategoryService _categoryService;

        public ProductService(IProductRepository productRepository)//, ICategoryService categoryService)
        {
            _productRepository = productRepository;
           // _categoryService = categoryService;
        }

        public Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProductAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int count)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> GetProductByIdAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<ProductDto>> GetProductsAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeProductId = null)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<ProductDto>> SearchProductsAsync(string searchTerm, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDto> UpdateProductAsync(Guid productId, UpdateProductDto updateProductDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateStockAsync(Guid productId, int quantity)
        {
            await _productRepository.UpdateStockAsync(productId, quantity);

          //  _categoryService.UpdateStock(Guid.NewGuid(), productId, quantity);

            return true;
        }
    }
}
