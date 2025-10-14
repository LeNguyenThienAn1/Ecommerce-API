using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.EntityHandler.Queries
{
    public interface IUserQueries
    {
        /// <summary>
        /// Lấy danh sách tất cả người dùng.
        /// </summary>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        /// <summary>
        /// Lấy thông tin chi tiết người dùng theo ID.
        /// </summary>
        Task<UserDto?> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Tìm kiếm người dùng theo tên hoặc email.
        /// </summary>
        Task<IEnumerable<UserDto>> SearchUsersAsync(string keyword);

        /// <summary>
        /// Lấy thông tin người dùng theo số điện thoại.
        /// </summary>
        Task<UserDto?> GetUserByPhoneNumberAsync(string phoneNumber);
    }
}
