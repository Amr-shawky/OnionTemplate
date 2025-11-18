using OnionTemplate.Core.Entities;
using OnionTemplate.Core.Enums;
using OnionTemplate.Application.Interfaces.Services;

namespace OnionTemplate.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, IAuthService authService)
        {
            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Electronics", Description = "Electronic devices and accessories", IsActive = true },
                    new Category { Name = "Clothing", Description = "Fashion and apparel", IsActive = true },
                    new Category { Name = "Books", Description = "Books and educational materials", IsActive = true },
                    new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies", IsActive = true },
                    new Category { Name = "Sports", Description = "Sports equipment and fitness gear", IsActive = true }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Users
            if (!context.Users.Any())
            {
                var adminPasswordHash = await authService.HashPasswordAsync("Admin123!");
                var users = new[]
                {
                    new User
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "admin@ecommerce.com",
                        PasswordHash = adminPasswordHash,
                        Role = UserRole.Admin,
                        IsEmailVerified = true
                    },
                    new User
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@example.com",
                        PasswordHash = await authService.HashPasswordAsync("User123!"),
                        Role = UserRole.Customer,
                        IsEmailVerified = true
                    }
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

            // Seed Products
            if (!context.Products.Any())
            {
                var electronicsCategory = context.Categories.First(c => c.Name == "Electronics");
                var clothingCategory = context.Categories.First(c => c.Name == "Clothing");
                var booksCategory = context.Categories.First(c => c.Name == "Books");

                var products = new[]
                {
                    new Product
                    {
                        Name = "iPhone 15 Pro",
                        Description = "Latest Apple iPhone with advanced features",
                        Price = 999.99m,
                        StockQuantity = 50,
                        SKU = "IPHONE15PRO",
                        CategoryId = electronicsCategory.Id,
                        Brand = "Apple",
                        Weight = 0.221m,
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Samsung Galaxy S24",
                        Description = "Premium Android smartphone",
                        Price = 899.99m,
                        StockQuantity = 75,
                        SKU = "GALAXYS24",
                        CategoryId = electronicsCategory.Id,
                        Brand = "Samsung",
                        Weight = 0.196m,
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Cotton T-Shirt",
                        Description = "Comfortable cotton t-shirt",
                        Price = 24.99m,
                        StockQuantity = 200,
                        SKU = "COTTON-TSHIRT",
                        CategoryId = clothingCategory.Id,
                        Brand = "BasicWear",
                        Weight = 0.15m,
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Programming Book",
                        Description = "Learn programming fundamentals",
                        Price = 49.99m,
                        StockQuantity = 100,
                        SKU = "PROG-BOOK-001",
                        CategoryId = booksCategory.Id,
                        Brand = "TechBooks",
                        Weight = 0.5m,
                        IsActive = true
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}

