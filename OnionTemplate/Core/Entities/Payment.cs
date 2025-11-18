using OnionTemplate.Core.Enums;

namespace OnionTemplate.Core.Entities
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? TransactionId { get; set; }
        public string? PaymentGatewayResponse { get; set; }
        public DateTime? ProcessedAt { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
    }
}

