using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product - Category (1-n)
            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");

                entity.HasOne<CategoryEntity>()
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Category
            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Description).HasMaxLength(500);
            });

            // Order
            modelBuilder.Entity<OrderEntity>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.Id);

                entity.Property(o => o.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(o => o.ShippingAddress).HasMaxLength(500);
                entity.Property(o => o.OrderDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // OrderDetail (bảng trung gian many-to-many Product <-> Order)
            modelBuilder.Entity<OrderDetailEntity>(entity =>
            {
                entity.ToTable("OrderDetails");
                entity.HasKey(od => new { od.OrderId, od.ProductId });

                entity.Property(od => od.Quantity).IsRequired();
                entity.Property(od => od.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(od => od.Order)
                      .WithMany(o => o.Details)
                      .HasForeignKey(od => od.OrderId);

                entity.HasOne(od => od.Product)
                      .WithMany()
                      .HasForeignKey(od => od.ProductId);
            });

            // User
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(200);
                entity.Property(u => u.PhoneNumber).HasMaxLength(20);
                entity.Property(u => u.Address).HasMaxLength(500);
                entity.Property(u => u.Role).HasMaxLength(50);
            });
        }

    }
}
