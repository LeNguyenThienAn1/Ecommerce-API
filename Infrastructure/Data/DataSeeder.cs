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
            if (!await context.Users.AnyAsync(u => u.Role == UserType.Admin))
            {
                var adminUser = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Email = "admin@example.com",
                    PhoneNumber = "+84987654321",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
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
