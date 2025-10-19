using System;

namespace Infrastructure
{
    public class UserEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; } 

        public string Password { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public string RejectReason { get; set; } = string.Empty;

        public UserType Role { get; set; } // Admin, Customer, etc.

        public string RefreshToken { get; set; } = string.Empty;
    }
}
