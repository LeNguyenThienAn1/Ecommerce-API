using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using Application.EntityHandler.Services;
using Application.Interfaces;
using Application.Interfaces.Queries;
using Infrastructure;
using Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

public class OrderService : IOrderService
{
    private readonly IProductQueries _productQueries;
    private readonly IOrderQueries _orderQueries;
    private readonly EcommerceDbContext _dbContext;
    private readonly IMomoService _momoService;

    public OrderService(
        IProductQueries productQueries,
        IOrderQueries orderQueries,
        EcommerceDbContext dbContext,
        IMomoService momoService)
    {
        _productQueries = productQueries;
        _orderQueries = orderQueries;
        _dbContext = dbContext;
        _momoService = momoService;
    }

    public async Task<bool> CreateOrderAsync(CreateOrderDto request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tạo đơn hàng mới (cho cả COD và MoMo)
    /// ✅ KHÔNG gọi MomoService ở đây nữa - để MomoController xử lý
    /// </summary>
    public async Task<Guid> CreateOrderAsync(CreateOrderDto dto, BillInfoDto billInfo)
    {
        var order = new OrderEntity
        {
            Id = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            CustomerName = billInfo.Name,
            ShippingAddress = billInfo.Address,
            PhoneNumber = billInfo.PhoneNumber,
            PaymentMethod = billInfo.PaymentMethod, // "COD" hoặc "MOMO"
            Note = billInfo.Note,
            Status = OrderStatus.Created, // Trạng thái ban đầu
            Details = new List<OrderDetailEntity>(),
            CreateBy = dto.BoughtBy,
            UpdateBy = dto.BoughtBy,
            UserId = dto.BoughtBy
        };

        decimal totalAmount = 0;

        foreach (var productId in dto.ProductIds)
        {
            var productEntity = await _productQueries.GetProductByIdAsync(productId);
            if (productEntity == null) continue;

            var orderDetail = new OrderDetailEntity
            {
                OrderId = order.Id,
                ProductId = productEntity.Id,
                Quantity = 1,
                UnitPrice = productEntity.Price
            };

            order.Details.Add(orderDetail);
            totalAmount += productEntity.Price;

            // ✅ Cập nhật trạng thái sản phẩm
            productEntity.Status = ProductStatus.Sold;
            productEntity.BoughtBy = dto.BoughtBy;
            _dbContext.Products.Update(productEntity);
        }

        // ✅ Lưu Order vào DB
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // ❌ XÓA đoạn gọi MomoService ở đây
        // Lý do: Frontend sẽ gọi riêng endpoint /api/Momo/create-payment với orderId

        return order.Id;
    }

    public async Task<bool> ApproveOrderAsync(Guid orderId)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order == null) return false;

        if (order.Status != OrderStatus.Created) return false;

        order.Status = OrderStatus.SellerConfirmed;
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RejectOrderAsync(Guid orderId)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order == null) return false;

        if (order.Status != OrderStatus.Created && order.Status != OrderStatus.SellerConfirmed)
            return false;

        order.Status = OrderStatus.Rejected;
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CancelOrderAsync(Guid orderId)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order == null) return false;

        if (order.Status != OrderStatus.Created) return false;

        order.Status = OrderStatus.FailedShipping;
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        return await _orderQueries.GetAllAsync();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
    {
        return await _orderQueries.GetByIdAsync(orderId);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(Guid userId)
    {
        return await _orderQueries.GetByUserAsync(userId);
    }

    /// <summary>
    /// Xác nhận thanh toán MoMo thành công (được gọi từ IPN callback)
    /// </summary>
    public async Task<bool> ConfirmPaymentSuccessAsync(Guid orderId, Guid userId)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return false;

        // ✅ Chỉ xác nhận thanh toán khi đơn hàng đang ở trạng thái 'Created' hoặc 'SellerConfirmed'
        if (order.Status != OrderStatus.Created && order.Status != OrderStatus.SellerConfirmed)
            return false;

        // ✅ Cập nhật trạng thái đơn hàng thành công
        order.Status = OrderStatus.Successfully;
        order.UpdateBy = userId; // Đánh dấu được cập nhật từ callback
        // Không cập nhật OrderDate vì đó là ngày tạo đơn

        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}