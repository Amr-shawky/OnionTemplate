using MediatR;

namespace OnionTemplate.CQRS.Products
{
     public record ProductDeletedEvent(Guid ProductID) : INotification;
}
