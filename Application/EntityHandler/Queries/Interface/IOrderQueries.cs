using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.EntityHandler.Queries.Interface
{
    /// <summary>
    /// Queries cho thao tác lấy dữ liệu đơn hàng (chỉ đọc)
    /// </summary>
    public interface IOrderQueries
    {
        /// <summary>
        /// Lấy chi tiết đơn hàng theo Id
        /// </summary>
        /// <param name="orderId">Id của đơn hàng</param>
        /// <returns>OrderDto hoặc null nếu không tìm thấy</returns>
        Task<OrderDto?> GetByIdAsync(Guid orderId);

        /// <summary>
        /// Lấy tất cả đơn hàng theo UserId (lịch sử mua hàng của User)
        /// </summary>
        /// <param name="userId">Id của User</param>
        /// <returns>Danh sách đơn hàng</returns>
        Task<IEnumerable<OrderDto>> GetByUserAsync(Guid userId);

        /// <summary>
        /// Lấy toàn bộ đơn hàng trong hệ thống
        /// </summary>
        /// <returns>Danh sách đơn hàng</returns>
        Task<IEnumerable<OrderDto>> GetAllAsync();
    }
}
