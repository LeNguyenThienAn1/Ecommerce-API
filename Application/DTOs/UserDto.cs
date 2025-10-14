using System;

namespace Application.DTOs
{
    // DTO chính cho CRUD (Admin quản lý user)
    public class UserDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Admin, Customer, etc.
        public bool IsActive { get; set; } = true;
    }
    public class ValidateUserProfileDto
    {
        public bool IsValid { get; set; }
        public List<string> Error { get; set; } = new();
    }

        // DTO rút gọn cho trang profile / đăng nhập
        //public class UserProfileDto 
        //{
        //    public Guid Id { get; set; }
        //    public string PhoneNumber { get; set; } = string.Empty;
        //    public string? Name { get; set; }
        //    public string? Role { get; set; }
        //    public string? Email { get; set; } // 👈 Thêm email cho trang profile
        //}
    }
