using OnionTemplate.Core.Entities;
using OnionTemplate.Core.Enums;

namespace OnionTemplate.Application.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserAsync(Guid userId);
        Task<Order?> GetOrderWithItemsAsync(Guid orderId);
        Task<Order?> GetOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<string> GenerateOrderNumberAsync();
    }
}

