using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnionTemplate.Core.Entities;

namespace OnionTemplate.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.ShippingCost)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.TaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Notes)
                .HasMaxLength(1000);

            builder.Property(o => o.ShippingFirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.ShippingLastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.ShippingAddressLine1)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.ShippingAddressLine2)
                .HasMaxLength(200);

            builder.Property(o => o.ShippingCity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.ShippingState)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.ShippingPostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.ShippingCountry)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(o => o.OrderNumber)
                .IsUnique();

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

