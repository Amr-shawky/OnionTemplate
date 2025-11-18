namespace OnionTemplate.Core.Entities
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? AltText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; } = false;

        // Navigation properties
        public Product Product { get; set; } = null!;
    }
}

