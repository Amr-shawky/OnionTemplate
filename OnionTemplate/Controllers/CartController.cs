using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using OnionTemplate.Application.DTOs.Cart;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;

namespace OnionTemplate.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CartController> _logger;

        public CartController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CartController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cartItems = await _unitOfWork.Cart.GetCartItemsByUserAsync(userId);
                var cartItemDtos = _mapper.Map<IEnumerable<CartItemDto>>(cartItems);

                var cart = new CartDto
                {
                    Items = cartItemDtos.ToList(),
                    TotalAmount = cartItemDtos.Sum(item => item.TotalPrice),
                    TotalItems = cartItemDtos.Sum(item => item.Quantity)
                };

                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartItemDto>> AddToCart(AddToCartDto addToCartDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Check if product exists and is active
                var product = await _unitOfWork.Products.GetByIdAsync(addToCartDto.ProductId);
                if (product == null || !product.IsActive)
                {
                    return BadRequest("Product not found or inactive");
                }

                // Check stock availability
                if (product.StockQuantity < addToCartDto.Quantity)
                {
                    return BadRequest("Insufficient stock");
                }

                // Check if item already exists in cart
                var existingCartItem = await _unitOfWork.Cart.GetCartItemAsync(userId, addToCartDto.ProductId);
                if (existingCartItem != null)
                {
                    // Update quantity
                    var newQuantity = existingCartItem.Quantity + addToCartDto.Quantity;
                    if (product.StockQuantity < newQuantity)
                    {
                        return BadRequest("Insufficient stock");
                    }

                    existingCartItem.Quantity = newQuantity;
                    await _unitOfWork.Cart.UpdateAsync(existingCartItem);
                }
                else
                {
                    // Add new item
                    var cartItem = _mapper.Map<CartItem>(addToCartDto);
                    cartItem.UserId = userId;
                    await _unitOfWork.Cart.AddAsync(cartItem);
                }

                await _unitOfWork.SaveChangesAsync();

                // Get updated cart item with product info
                var updatedCartItem = await _unitOfWork.Cart.GetCartItemAsync(userId, addToCartDto.ProductId);
                var cartItemDto = _mapper.Map<CartItemDto>(updatedCartItem);

                _logger.LogInformation("Item added to cart: {UserId}, {ProductId}", userId, addToCartDto.ProductId);
                return Ok(cartItemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart: {UserId}, {ProductId}", GetCurrentUserId(), addToCartDto.ProductId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("items/{productId}")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItem(Guid productId, UpdateCartItemDto updateCartItemDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var cartItem = await _unitOfWork.Cart.GetCartItemAsync(userId, productId);
                
                if (cartItem == null)
                {
                    return NotFound();
                }

                // Check stock availability
                if (cartItem.Product.StockQuantity < updateCartItemDto.Quantity)
                {
                    return BadRequest("Insufficient stock");
                }

                cartItem.Quantity = updateCartItemDto.Quantity;
                await _unitOfWork.Cart.UpdateAsync(cartItem);
                await _unitOfWork.SaveChangesAsync();

                var cartItemDto = _mapper.Map<CartItemDto>(cartItem);

                _logger.LogInformation("Cart item updated: {UserId}, {ProductId}", userId, productId);
                return Ok(cartItemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item: {UserId}, {ProductId}", GetCurrentUserId(), productId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveFromCart(Guid productId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var cartItem = await _unitOfWork.Cart.GetCartItemAsync(userId, productId);
                
                if (cartItem == null)
                {
                    return NotFound();
                }

                await _unitOfWork.Cart.DeleteAsync(cartItem.Id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Item removed from cart: {UserId}, {ProductId}", userId, productId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart: {UserId}, {ProductId}", GetCurrentUserId(), productId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _unitOfWork.Cart.ClearCartAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cart cleared: {UserId}", userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart: {UserId}", GetCurrentUserId());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCartItemCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var count = await _unitOfWork.Cart.GetCartItemCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item count: {UserId}", GetCurrentUserId());
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

