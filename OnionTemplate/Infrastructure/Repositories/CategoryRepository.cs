using Microsoft.EntityFrameworkCore;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;
using OnionTemplate.Infrastructure.Data;

namespace OnionTemplate.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetParentCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive && c.ParentCategoryId == null)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentCategoryId)
        {
            return await _dbSet
                .Where(c => c.IsActive && c.ParentCategoryId == parentCategoryId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithProductsAsync(Guid categoryId)
        {
            return await _dbSet
                .Include(c => c.Products.Where(p => p.IsActive))
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }
    }
}

