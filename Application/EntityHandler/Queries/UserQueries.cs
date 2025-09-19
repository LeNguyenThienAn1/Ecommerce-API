using Application.Interfaces.Queries;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly EcommerceDbContext _context;

        public UserQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<UserEntity> GetUserByPhoneNumber(string phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }
    }
}
