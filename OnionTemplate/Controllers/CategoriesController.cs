using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;
using OnionTemplate.Application.Interfaces.Services;

namespace OnionTemplate.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CategoriesController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("parent")]
        public async Task<ActionResult<IEnumerable<Category>>> GetParentCategories()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetParentCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting parent categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/subcategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetSubCategories(Guid id)
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetSubCategoriesAsync(id);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subcategories for category: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(Guid id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetCategoryWithProductsAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            try
            {
                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category created successfully: {CategoryId}", category.Id);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Category>> UpdateCategory(Guid id, Category category)
        {
            try
            {
                if (id != category.Id)
                {
                    return BadRequest();
                }

                var existingCategory = await _unitOfWork.Categories.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound();
                }

                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.ImageUrl = category.ImageUrl;
                existingCategory.IsActive = category.IsActive;
                existingCategory.ParentCategoryId = category.ParentCategoryId;

                await _unitOfWork.Categories.UpdateAsync(existingCategory);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category updated successfully: {CategoryId}", id);
                return Ok(existingCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }

                // Check if category has products
                var categoryWithProducts = await _unitOfWork.Categories.GetCategoryWithProductsAsync(id);
                if (categoryWithProducts?.Products.Any() == true)
                {
                    return BadRequest("Cannot delete category that contains products");
                }

                // Check if category has subcategories
                var subCategories = await _unitOfWork.Categories.GetSubCategoriesAsync(id);
                if (subCategories.Any())
                {
                    return BadRequest("Cannot delete category that has subcategories");
                }

                await _unitOfWork.Categories.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Category deleted successfully: {CategoryId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category: {CategoryId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

