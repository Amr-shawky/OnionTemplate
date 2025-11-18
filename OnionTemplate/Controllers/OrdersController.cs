using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using OnionTemplate.Application.DTOs.Common;
using OnionTemplate.Application.DTOs.Order;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Core.Entities;
using OnionTemplate.Core.Enums;
using OnionTemplate.Application.Interfaces.Services;
using MediatR;

namespace OnionTemplate.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderService _orderService;
        private readonly IMediator _mediator;

        public OrdersController(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OrdersController> logger,
            IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _orderService = orderService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<OrderDto>>> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                IEnumerable<Order> orders;

                if (IsManager())
                {
                    // Managers and admins can see all orders
                    orders = await _unitOfWork.Orders.GetAllAsync();
                }
                else
                {
                    // Regular users can only see their own orders
                    orders = await _unitOfWork.Orders.GetOrdersByUserAsync(userId);
                }

                var totalCount = orders.Count();
                var paginatedOrders = orders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(paginatedOrders);

                var result = new PaginatedResult<OrderDto>
                {
                    Data = orderDtos,
                    Pagination = new PaginationDto
                    {
                        Page = page,
                        PageSize = pageSize,
                        TotalCount = totalCount
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                var userId = GetCurrentUserId();
                // Check if user owns the order or is a manager
                if (order.UserId != userId && !IsManager())
                {
                    return Forbid();
                }

                var orderDto = _mapper.Map<OrderDto>(order);
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order: {OrderId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Get cart items
                var cartItems = await _unitOfWork.Cart.GetCartItemsByUserAsync(userId);
                if (!cartItems.Any())
                {
                    return BadRequest("Cart is empty");
                }

                 

                return await _orderService.CreateOrderAsync(userId, createOrderDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating order for user: {UserId}", GetCurrentUserId());
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderStatus status)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                order.Status = status;
                
                if (status == OrderStatus.Shipped)
                {
                    order.ShippedAt = DateTime.UtcNow;
                }
                else if (status == OrderStatus.Delivered)
                {
                    order.DeliveredAt = DateTime.UtcNow;
                }

                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Order status updated: {OrderId}, Status: {Status}", id, status);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status: {OrderId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                var userId = GetCurrentUserId();
                // Check if user owns the order or is a manager
                if (order.UserId != userId && !IsManager())
                {
                    return Forbid();
                }

                // Only allow cancellation of pending or confirmed orders
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                {
                    return BadRequest("Order cannot be cancelled");
                }

                order.Status = OrderStatus.Cancelled;
                await _unitOfWork.Orders.UpdateAsync(order);

                // Restore stock
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        await _unitOfWork.Products.UpdateStockAsync(orderItem.ProductId, 
                            product.StockQuantity + orderItem.Quantity);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Order cancelled: {OrderId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order: {OrderId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

