using AutoMapper;
using OnionTemplate.Application.DTOs.Common;
using OnionTemplate.Application.DTOs.Order;
using OnionTemplate.Application.Interfaces;
using OnionTemplate.Application.Interfaces.Services;
using OnionTemplate.Core.Entities;
using OnionTemplate.Core.Enums;

namespace OnionTemplate.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IProductService productService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
            _mapper = mapper;
        }

        public Task<decimal> CalculateOrderTotalAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CancelOrderAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto createOrderDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var cartItems = await _unitOfWork.Cart.GetCartItemsByUserAsync(userId);

                // Create order
                var order = _mapper.Map<Order>(createOrderDto);
                order.UserId = userId;
                order.OrderNumber = await _unitOfWork.Orders.GenerateOrderNumberAsync();
                order.Status = OrderStatus.Pending;
                order.PaymentStatus = PaymentStatus.Pending;

                // Calculate totals
                decimal subtotal = 0;
                var orderItems = new List<OrderItem>();

                foreach (var cartItem in cartItems)
                {
                    // Check stock availability
                    if (cartItem.Product.StockQuantity < cartItem.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product: {cartItem.Product.Name}");
                    }

                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price,
                        TotalPrice = cartItem.Quantity * cartItem.Product.Price
                    };

                    orderItems.Add(orderItem);
                    subtotal += orderItem.TotalPrice;

                    // Update stock
                    //await _unitOfWork.Products.UpdateStockAsync(cartItem.ProductId,
                    //    cartItem.Product.StockQuantity - cartItem.Quantity);

                    _productService.UpdateStockAsync(cartItem.ProductId,
                        cartItem.Product.StockQuantity - cartItem.Quantity);

                    //_unitOfWork.Categories.UpdateStock(categoryID cartItem.ProductId, cartItem.Quantity);


                }

                // Calculate shipping and tax (simplified calculation)
                order.ShippingCost = 10.00m; // Fixed shipping cost
                order.TaxAmount = subtotal * 0.1m; // 10% tax
                order.TotalAmount = subtotal + order.ShippingCost + order.TaxAmount;

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // Add order items
                foreach (var orderItem in orderItems)
                {
                    orderItem.OrderId = order.Id;
                    await _unitOfWork.Orders.GetByIdAsync(order.Id); // This adds the OrderItem through EF navigation
                }

                // Clear cart
                await _unitOfWork.Cart.ClearCartAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Get created order with items
                var createdOrder = await _unitOfWork.Orders.GetOrderWithItemsAsync(order.Id);
                var orderDto = _mapper.Map<OrderDto>(createdOrder);

                // _logger.LogInformation("Order created successfully: {OrderId}", order.Id);
                return orderDto;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public Task<OrderDto> GetOrderByIdAsync(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto> GetOrderByNumberAsync(string orderNumber)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<OrderDto>> GetOrdersAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<OrderDto>> GetOrdersByUserAsync(Guid userId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
