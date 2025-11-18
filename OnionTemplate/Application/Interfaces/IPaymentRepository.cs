using OnionTemplate.Core.Entities;

namespace OnionTemplate.Application.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment?> GetPaymentByOrderAsync(Guid orderId);
        Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId);
        Task<IEnumerable<Payment>> GetPaymentsByUserAsync(Guid userId);
    }
}

