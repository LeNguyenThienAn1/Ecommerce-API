using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.EntityHandler.Services
{
    /// <summary>
    /// Định nghĩa các nghiệp vụ liên quan đến xử lý đơn hàng
    /// </summary>
    public interface IOrderService 
    {
        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        Task<Guid> CreateOrderAsync(CreateOrderDto dto, BillInfoDto billInfo);

        /// <summary>
        /// Duyệt đơn hàng (Admin)
        /// </summary>
        Task<bool> ApproveOrderAsync(Guid orderId);

        /// <summary>
        /// Từ chối đơn hàng (Admin)
        /// </summary>
        Task<bool> RejectOrderAsync(Guid orderId);

        /// <summary>
        /// Người dùng hủy đơn hàng (chỉ khi chưa được admin duyệt)
        /// </summary>
        Task<bool> CancelOrderAsync(Guid orderId);

        /// <summary>
        /// Lấy toàn bộ đơn hàng trong hệ thống
        /// (dành cho Admin)
        /// </summary>
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

        /// <summary>
        /// Lấy chi tiết 1 đơn hàng theo Id
        /// </summary>
        Task<OrderDto?> GetOrderByIdAsync(Guid orderId);

        /// <summary>
        /// Lấy danh sách đơn hàng theo UserId (lịch sử mua hàng)
        /// </summary>
        Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(Guid userId);
        Task<bool> ConfirmPaymentSuccessAsync(Guid orderId, Guid userId);

    }
}
