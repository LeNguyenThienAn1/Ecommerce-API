namespace Application.DTOs
{
    public class AuthDto
    {
        public class RegisterRequest
        {
            public string PhoneNumber { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string? Name { get; set; }
        }

        public class LoginRequest
        {
            public string PhoneNumber { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class RefreshTokenRequest
        {
            public string RefreshToken { get; set; } = string.Empty;
        }

        public class LogoutRequest
        {
            public string RefreshToken { get; set; } = string.Empty;
        }

        public class ChangePasswordRequest
        {
            public string OldPassword { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
            public string ConfirmNewPassword { get; set; } = string.Empty;
        }

        public class AuthResult
        {
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public UserProfileDto User { get; set; } = new();
        }
    }

    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Role { get; set; }
    }
}
