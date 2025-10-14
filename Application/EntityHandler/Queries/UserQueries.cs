using Application.DTOs;
using Application.EntityHandler.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly EcommerceDbContext _context;

        public UserQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive, // nếu có trong entity
                    AvatarUrl = u.AvatarUrl // nếu có trong entity
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    AvatarUrl = u.AvatarUrl
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserDto>> SearchUsersAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<UserDto>();

            keyword = keyword.ToLower();

            return await _context.Users
                .Where(u => u.Name.ToLower().Contains(keyword) || u.Email.ToLower().Contains(keyword))
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    AvatarUrl = u.AvatarUrl
                })
                .ToListAsync();
        }
        public async Task<UserDto?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            return await _context.Users
                .Where(u => u.PhoneNumber == phoneNumber)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    AvatarUrl = u.AvatarUrl
                })
                .FirstOrDefaultAsync();
        }

    }
}
