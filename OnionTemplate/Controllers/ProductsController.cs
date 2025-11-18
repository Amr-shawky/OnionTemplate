using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using OnionTemplate.Application.DTOs.Common;
using OnionTemplate.Application.DTOs.Product;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;
using MediatR;
using OnionTemplate.CQRS.Products.Queries;
using OnionTemplate.CQRS.Products.Commands;
using OnionTemplate.Application.Interfaces.Services;
using OnionTemplate.MessageBroker.Messages;

namespace OnionTemplate.Controllers
{
    //[Route("{controller}/{action}")]
    public class ProductsController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;
        private readonly IMediator _mediator;
        private readonly IRabbitMQPublisherService _rabbitMQPublisherService;

        public ProductsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductsController> logger,
            IMediator mediator,
            IRabbitMQPublisherService rabbitMQPublisherService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
            _rabbitMQPublisherService = rabbitMQPublisherService;
        }


        [HttpPost]
        public async Task PublishMessage()
        {
            ProductCreatedMessage productDto = new ()
            {
                Brand = "Dell",
                Name = "Laptop",
                Price = 571244,
                Date = DateTime.Now,
                Type= "ProductCreatedMessage",
            };

            var message = System.Text.Json.JsonSerializer.Serialize(productDto);
            
            await _rabbitMQPublisherService.PublishMessage("Amazon_Orders", "", message);
        }

        //[HttpGet]
        //public async Task<ProductDto> GetByID(Guid productID)
        //{
        //    return await _mediator.Send(new GetProductByIDQuery(productID));   
        //}

        //[HttpGet]
        //public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts(
        //    [FromQuery] int page = 1,
        //    [FromQuery] int pageSize = 10,
        //    [FromQuery] string? search = null,
        //    [FromQuery] Guid? categoryId = null,
        //    [FromQuery] decimal? minPrice = null,
        //    [FromQuery] decimal? maxPrice = null)
        //{
        //    try
        //    {
        //        IEnumerable<Product> products;

        //        if (!string.IsNullOrEmpty(search))
        //        {
        //            products = await _unitOfWork.Products.SearchProductsAsync(search);
        //        }
        //        else if (categoryId.HasValue)
        //        {
        //            products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId.Value);
        //        }
        //        else if (minPrice.HasValue && maxPrice.HasValue)
        //        {
        //            products = await _unitOfWork.Products.GetProductsByPriceRangeAsync(minPrice.Value, maxPrice.Value);
        //        }
        //        else
        //        {
        //            products = await _unitOfWork.Products.GetActiveProductsAsync();
        //        }

        //        var totalCount = products.Count();
        //        var paginatedProducts = products
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize);

        //        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(paginatedProducts);

        //        var result = new PaginatedResult<ProductDto>
        //        {
        //            Data = productDtos,
        //            Pagination = new PaginationDto
        //            {
        //                Page = page,
        //                PageSize = pageSize,
        //                TotalCount = totalCount
        //            }
        //        };

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting products");
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        //{
        //    try
        //    {
        //        var product = await _unitOfWork.Products.GetProductWithImagesAsync(id);
        //        if (product == null)
        //        {
        //            return NotFound();
        //        }

        //        var productDto = _mapper.Map<ProductDto>(product);
        //        return Ok(productDto);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting product: {ProductId}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //[HttpGet("featured")]
        //public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts([FromQuery] int count = 8)
        //{
        //    try
        //    {
        //        var products = await _unitOfWork.Products.GetFeaturedProductsAsync(count);
        //        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        //        return Ok(productDtos);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting featured products");
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        ////[HttpPost]
        ////[Authorize(Roles = "Admin,Manager")]
        ////public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        //{
        //    try
        //    {
        //        var id = await _mediator.Send(new AddProductCommand(createProductDto.Name, createProductDto.Price));


        //        // Check if SKU is unique
        //        var isSkuUnique = await _unitOfWork.Products.IsSkuUniqueAsync(createProductDto.SKU);
        //        if (!isSkuUnique)
        //        {
        //            return BadRequest("SKU already exists");
        //        }

        //        var product = _mapper.Map<Product>(createProductDto);
        //        await _unitOfWork.Products.AddAsync(product);
        //        await _unitOfWork.SaveChangesAsync();

        //        // Get the created product with category info
        //        var createdProduct = await _unitOfWork.Products.GetProductWithImagesAsync(product.Id);
        //        var productDto = _mapper.Map<ProductDto>(createdProduct);

        //        _logger.LogInformation("Product created successfully: {ProductId}", product.Id);
        //        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating product");
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //[HttpPut("{id}")]
        //[Authorize(Roles = "Admin,Manager")]
        //public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, UpdateProductDto updateProductDto)
        //{
        //    try
        //    {
        //        var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
        //        if (existingProduct == null)
        //        {
        //            return NotFound();
        //        }

        //        _mapper.Map(updateProductDto, existingProduct);
        //        await _unitOfWork.Products.UpdateAsync(existingProduct);
        //        await _unitOfWork.SaveChangesAsync();

        //        var updatedProduct = await _unitOfWork.Products.GetProductWithImagesAsync(id);
        //        var productDto = _mapper.Map<ProductDto>(updatedProduct);

        //        _logger.LogInformation("Product updated successfully: {ProductId}", id);
        //        return Ok(productDto);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating product: {ProductId}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Admin,Manager")]
        //public async Task<IActionResult> DeleteProduct(Guid id)
        //{

        //    try
        //    {
        //        var product = await _unitOfWork.Products.GetByIdAsync(id);
        //        if (product == null)
        //        {
        //            return NotFound();
        //        }

        //        await _unitOfWork.Products.DeleteAsync(id);
        //        await _unitOfWork.SaveChangesAsync();

        //        _logger.LogInformation("Product deleted successfully: {ProductId}", id);
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting product: {ProductId}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}
    }
}

