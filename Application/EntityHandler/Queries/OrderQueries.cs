using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using Infrastructure;
using Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Queries
{
    /// <summary>
    /// Triển khai các phương thức lấy dữ liệu đơn hàng (queries)
    /// </summary>
    public class OrderQueries : IOrderQueries
    {
        private readonly EcommerceDbContext _db;

        public OrderQueries(EcommerceDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng theo Id
        /// </summary>
        public async Task<OrderDto?> GetByIdAsync(Guid orderId)
        {
            var order = await _db.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;

            return MapToDto(order);
        }

        /// <summary>
        /// Lấy toàn bộ đơn hàng của một User (lịch sử mua hàng)
        /// </summary>
        public async Task<IEnumerable<OrderDto>> GetByUserAsync(Guid userId)
        {
            var orders = await _db.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Product)
                .Where(o => o.UserId == userId) 
                .ToListAsync();

            return orders.Select(MapToDto);
        }

        /// <summary>
        /// Lấy toàn bộ đơn hàng trong hệ thống
        /// </summary>
        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _db.Orders
                .Include(o => o.Details)
                .ThenInclude(d => d.Product)
                .ToListAsync();

            return orders.Select(MapToDto);
        }

        /// <summary>
        /// Chuyển đổi OrderEntity sang OrderDto
        /// </summary>
        private static OrderDto MapToDto(OrderEntity order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Address = order.ShippingAddress,
                PhoneNumber = order.PhoneNumber,
                PaymentMethod = order.PaymentMethod,
                Note = order.Note,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                Status = order.Status,
                Items = order.Details.Select(d => new OrderItemDto
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product?.Name ?? string.Empty,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity
                }).ToList()
            };
        }
    }
}
