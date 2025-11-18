namespace OnionTemplate.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal Weight { get; set; }
        public string? Brand { get; set; }

        // Foreign Keys
        public Guid CategoryId { get; set; }

        // Navigation properties
        public Category Category { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}

