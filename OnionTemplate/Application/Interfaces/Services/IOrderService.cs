using OnionTemplate.Application.DTOs.Common;
using OnionTemplate.Application.DTOs.Order;
using OnionTemplate.Core.Enums;

namespace OnionTemplate.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto createOrderDto);
        Task<OrderDto> GetOrderByIdAsync(Guid orderId);
        Task<OrderDto> GetOrderByNumberAsync(string orderNumber);
        Task<PaginatedResult<OrderDto>> GetOrdersByUserAsync(Guid userId, int page, int pageSize);
        Task<PaginatedResult<OrderDto>> GetOrdersAsync(int page, int pageSize);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
        Task<bool> CancelOrderAsync(Guid orderId);
        Task<decimal> CalculateOrderTotalAsync(Guid userId);
    }
}

