using OnionTemplate.Core.Entities;

namespace OnionTemplate.Application.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<IEnumerable<Category>> GetParentCategoriesAsync();
        Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentCategoryId);
        Task<Category?> GetCategoryWithProductsAsync(Guid categoryId);
    }
}

