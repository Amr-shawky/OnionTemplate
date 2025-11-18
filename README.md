# E-Commerce Application - Onion Architecture

A complete e-commerce application built using **Onion Architecture** (Clean Architecture) with ASP.NET Core 8.0.

## 🏗️ Architecture Overview

This application follows the Onion Architecture pattern with clear separation of concerns:

### Layers:

1. **Core Domain Layer** (`Core/`)
   - Entities (Product, User, Order, Category, etc.)
   - Enums (UserRole, OrderStatus, PaymentStatus, etc.)
   - No dependencies on external frameworks

2. **Application Layer** (`Application/`)
   - Interfaces for repositories and services
   - DTOs (Data Transfer Objects)
   - AutoMapper profiles
   - Business logic contracts

3. **Infrastructure Layer** (`Infrastructure/`)
   - Entity Framework DbContext and configurations
   - Repository implementations
   - External service implementations (Authentication, etc.)
   - Database access and external integrations

4. **Presentation Layer** (`Controllers/`)
   - API Controllers
   - Request/Response handling
   - Authentication and authorization

## 🚀 Features

- **User Management**: Registration, authentication with JWT tokens
- **Product Catalog**: Product browsing, searching, filtering by category/price
- **Shopping Cart**: Add/remove items, update quantities
- **Order Management**: Place orders, track status, cancel orders
- **Category Management**: Hierarchical categories
- **Admin Features**: Product and order management
- **Authentication & Authorization**: JWT-based with role-based access

## 🛠️ Technologies Used

- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Mapping**: AutoMapper
- **Logging**: Serilog
- **Password Hashing**: BCrypt
- **API Documentation**: Swagger/OpenAPI

## 📦 Key Packages

- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.AspNetCore.Authentication.JwtBearer
- AutoMapper.Extensions.Microsoft.DependencyInjection
- BCrypt.Net-Next
- Serilog.AspNetCore
- Swashbuckle.AspNetCore

## 🗄️ Database Schema

The application includes the following main entities:
- Users (with role-based access)
- Products (with categories and images)
- Orders (with order items and payment tracking)
- Categories (with hierarchical support)
- Cart Items
- Addresses
- Payments

## 🔐 Default Users

After seeding, the following users are available:

**Admin User:**
- Email: `admin@ecommerce.com`
- Password: `Admin123!`
- Role: Admin

**Regular User:**
- Email: `john.doe@example.com`
- Password: `User123!`
- Role: Customer

## 🚀 Getting Started

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd OnionTemplate
   ```

2. **Update connection string** in `appsettings.json`
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceDB;Trusted_Connection=true;MultipleActiveResultSets=true"
   }
   ```

3. **Run migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   Navigate to `https://localhost:7xxx/swagger` to explore the API

## 📚 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login

### Products
- `GET /api/products` - Get products (with filtering/search)
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/featured` - Get featured products
- `POST /api/products` - Create product (Admin/Manager)
- `PUT /api/products/{id}` - Update product (Admin/Manager)
- `DELETE /api/products/{id}` - Delete product (Admin/Manager)

### Cart
- `GET /api/cart` - Get user's cart
- `POST /api/cart/items` - Add item to cart
- `PUT /api/cart/items/{productId}` - Update cart item
- `DELETE /api/cart/items/{productId}` - Remove item from cart
- `DELETE /api/cart` - Clear cart

### Orders
- `GET /api/orders` - Get orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create order
- `PUT /api/orders/{id}/status` - Update order status (Admin/Manager)
- `POST /api/orders/{id}/cancel` - Cancel order

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/parent` - Get parent categories
- `GET /api/categories/{id}/subcategories` - Get subcategories
- `POST /api/categories` - Create category (Admin/Manager)
- `PUT /api/categories/{id}` - Update category (Admin/Manager)
- `DELETE /api/categories/{id}` - Delete category (Admin/Manager)

## 🔒 Security Features

- JWT token-based authentication
- Role-based authorization (Customer, Manager, Admin)
- Password hashing with BCrypt
- CORS configuration
- Input validation

## 📁 Project Structure

```
OnionTemplate/
├── Core/
│   ├── Entities/           # Domain entities
│   └── Enums/             # Domain enumerations
├── Application/
│   ├── DTOs/              # Data Transfer Objects
│   ├── Interfaces/        # Repository and service contracts
│   └── Mappings/          # AutoMapper profiles
├── Infrastructure/
│   ├── Data/              # EF DbContext and configurations
│   ├── Repositories/      # Repository implementations
│   └── Services/          # Service implementations
└── Controllers/           # API Controllers
```

## 🧪 Testing the API

Use the provided Swagger UI or tools like Postman to test the API endpoints. Start by:

1. Register a new user or use the seeded admin account
2. Login to get a JWT token
3. Include the token in the Authorization header: `Bearer <your-token>`
4. Explore the various endpoints

## 🔧 Configuration

Key configuration sections in `appsettings.json`:

- **ConnectionStrings**: Database connection
- **JwtSettings**: JWT token configuration
- **Serilog**: Logging configuration

## 📈 Future Enhancements

- Payment gateway integration
- Email notifications
- Product reviews and ratings
- Inventory management
- Reporting and analytics
- File upload for product images
- Real-time notifications

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

