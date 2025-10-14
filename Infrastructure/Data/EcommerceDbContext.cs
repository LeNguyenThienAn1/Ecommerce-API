using Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
            : base(options)
        {
        }

        // 🧩 DbSets
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<BrandEntity> Brands { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderDetailEntity> OrderDetails { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<WishlistEntity> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== PRODUCT ==========
            modelBuilder.Entity<ProductEntity>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)");

                // Quan hệ: Product - Category
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ: Product - Brand
                entity.HasOne(p => p.Brand)
                      .WithMany(b => b.Products)
                      .HasForeignKey(p => p.BrandId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Map ProductDetail thành JSON (PostgreSQL JSONB)
                entity.Property(p => p.Detail)
                      .HasColumnType("jsonb")
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                          v => JsonSerializer.Deserialize<ProductDetail>(v, (JsonSerializerOptions)null)
                      )
                      .HasDefaultValueSql("'{}'::jsonb");
            });

            // ========== CATEGORY ==========
            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.Description)
                      .HasMaxLength(500);
            });

            // ========== BRAND ==========
            modelBuilder.Entity<BrandEntity>(entity =>
            {
                entity.ToTable("Brands");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(b => b.LogoUrl)
                      .HasMaxLength(500);
            });

            // ========== ORDER ==========
            modelBuilder.Entity<OrderEntity>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.Id);

                entity.Property(o => o.CustomerName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(o => o.ShippingAddress)
                      .HasMaxLength(500);

                entity.Property(o => o.OrderDate)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // ========== ORDER DETAIL ==========
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

            // ========== USER ==========
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(u => u.Email)
                      .HasMaxLength(200);

                entity.HasIndex(u => u.Email)
                      .IsUnique(); // Email duy nhất trong hệ thống

                entity.Property(u => u.Password)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(u => u.PhoneNumber)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasIndex(u => u.PhoneNumber)
                      .IsUnique(); // Số điện thoại duy nhất

                entity.Property(u => u.Address)
                      .HasMaxLength(500);

                entity.Property(u => u.AvatarUrl)
                      .HasMaxLength(500);

                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);

                entity.Property(u => u.RejectReason)
                      .HasMaxLength(500);

                entity.Property(u => u.Role)
                      .HasConversion<string>() // Enum UserType lưu dạng string
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(u => u.RefreshToken)
                      .HasMaxLength(500);
            });


        }
    }
}
