using Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAdminUserAsync(EcommerceDbContext context)
        {
            // Kiểm tra xem đã có admin nào chưa
            if (!await context.Users.AnyAsync(u => u.Role == UserType.Admin))
            {
                // Nếu chưa có, tạo một admin mới
                var adminUser = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Email = "admin@example.com",
                    PhoneNumber = "+84987654321", // Số điện thoại để đăng nhập
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    // Mật khẩu đã được mã hóa
                    Role = UserType.Admin,
                    IsActive = true,
                    Address = "Quản trị viên",
                    AvatarUrl = string.Empty,
                    RefreshToken = string.Empty,
                    CreateAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
