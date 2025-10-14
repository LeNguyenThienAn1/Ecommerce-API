using Application.DTOs;
using Application.Interfaces.Services;
using Infrastructure;
using Infrastructure.Entity; // Nếu UserEntity nằm trong namespace này
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly EcommerceDbContext _context;

        public UserService(EcommerceDbContext context)
        {
            _context = context;
        }

        // 🟩 CREATE
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Email))
                throw new ArgumentException("Email không được để trống.");

            // Kiểm tra trùng Email hoặc SĐT
            bool isEmailExists = await _context.Users.AnyAsync(u => u.Email == userDto.Email);
            if (isEmailExists)
                throw new InvalidOperationException("Email đã tồn tại.");

            bool isPhoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == userDto.PhoneNumber);
            if (isPhoneExists)
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");

            var entity = new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                Address = userDto.Address,
                AvatarUrl = userDto.AvatarUrl ?? string.Empty,
                IsActive = true,
                Password = "123456", // TODO: mã hóa hoặc thay đổi logic tạo mật khẩu
                Role = Enum.TryParse<UserType>(userDto.Role, true, out var role) ? role : UserType.Customer,
                RefreshToken = string.Empty
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }

        // 🟨 UPDATE
        public async Task<UserDto?> UpdateUserAsync(UserDto userDto)
        {
            var entity = await _context.Users.FindAsync(userDto.Id);
            if (entity == null)
                return null;

            var validateResult = await ValidateUserProfile(userDto.Email, userDto.PhoneNumber, userDto.Id);
            if (!validateResult.IsValid)
            {
                throw new InvalidOperationException(string.Join("; ", validateResult.Error));
            }

            entity.Name = userDto.Name ?? entity.Name;
            entity.Email = userDto.Email ?? entity.Email;
            entity.PhoneNumber = userDto.PhoneNumber ?? entity.PhoneNumber;
            entity.Address = userDto.Address ?? entity.Address;
            entity.AvatarUrl = userDto.AvatarUrl ?? entity.AvatarUrl;
            entity.Role = Enum.TryParse<UserType>(userDto.Role, true, out var role) ? role : entity.Role;
            entity.IsActive = userDto.IsActive;

            _context.Users.Update(entity);
            await _context.SaveChangesAsync();

            return MapToDto(entity);
        }
        private async Task<ValidateUserProfileDto> ValidateUserProfile(string email, string phoneNumber, Guid? excludeUserId = null)
        {
            var result = new ValidateUserProfileDto();
            var isEmailUnique = !await _context.Users.AnyAsync(u => u.Email == email && u.Id != excludeUserId);
            if (!isEmailUnique)
            {
                result.Error.Add("Email đã tồn tại.");
            }
            var isPhoneNumberUnique = !await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber && u.Id != excludeUserId);
            if (!isPhoneNumberUnique)
            {
                result.Error.Add("Số điện thoại đã tồn tại.");
            }
            if (result.Error.Count > 0)
            {
                result.IsValid = false;
            }
            else
            {
                result.IsValid = true;
            }
            return result;
        }

        // 🟥 DELETE
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null)
                return false;

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // 🟦 CHANGE STATUS
        public async Task<bool> ChangeUserStatusAsync(Guid id, bool isActive)
        {
            var entity = await _context.Users.FindAsync(id);
            if (entity == null)
                return false;

            entity.IsActive = isActive;
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        // 🟢 ACTIVATE
        public async Task<bool> ActivateUserAsync(Guid id)
        {
            return await ChangeUserStatusAsync(id, true);
        }

        // 🔴 DEACTIVATE
        public async Task<bool> DeactivateUserAsync(Guid id)
        {
            return await ChangeUserStatusAsync(id, false);
        }

        // 🧩 Helper mapping
        private static UserDto MapToDto(UserEntity user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role.ToString(),
                IsActive = user.IsActive
            };
        }
    }
}
