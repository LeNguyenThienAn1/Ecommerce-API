using Application.DTOs;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Tạo mới người dùng.
        /// </summary>
        Task<UserDto> CreateUserAsync(UserDto userDto);

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        Task<UserDto?> UpdateUserAsync(UserDto userDto);

        /// <summary>
        /// Xóa người dùng theo ID.
        /// </summary>
        Task<bool> DeleteUserAsync(Guid id);

        /// <summary>
        /// Thay đổi trạng thái hoạt động của người dùng (Active / Inactive).
        /// </summary>
        Task<bool> ChangeUserStatusAsync(Guid id, bool isActive);

        /// <summary>
        /// Kích hoạt người dùng (bật IsActive = true).
        /// </summary>
        Task<bool> ActivateUserAsync(Guid id);

        /// <summary>
        /// Vô hiệu hóa người dùng (tắt IsActive = false).
        /// </summary>
        Task<bool> DeactivateUserAsync(Guid id);
    }
}
