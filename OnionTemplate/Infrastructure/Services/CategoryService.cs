using OnionTemplate.Application.Interfaces;
using OnionTemplate.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace OnionTemplate.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        ICategoryRepository _categoryRepository;
        IProductService _productService;
        IUnitOfWork _unitOfWork;
        public CategoryService(ICategoryRepository categoryRepository, IProductService productService) 
        {
            _categoryRepository = categoryRepository;
            _productService = productService;
        }

        public async Task<bool> Delete(Guid categoryId)
        {
            await _categoryRepository.DeleteAsync(categoryId);

            //_productService.DelteProductsWithcategoryID(categoryId);

            //var products = (await _categoryRepository.GetCategoryWithProductsAsync(categoryId)).Products;

            //foreach (var product in products)
            //{
            //    await _unitOfWork.Products.DeleteAsync(product.Id);
            //}


            return true;
        }

        public void UpdateStock(Guid catgoeryId, Guid productId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
