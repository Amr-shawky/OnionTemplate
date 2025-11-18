using OnionTemplate.Core.Enums;

namespace OnionTemplate.Core.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
        public bool IsEmailVerified { get; set; } = false;
        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}

