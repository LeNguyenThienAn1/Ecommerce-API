using Application.DTOs;
using Application.EntityHandler.Services;
using Application.EntityHandler.Queries.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderQueries _orderQueries;

        public OrderController(IOrderService orderService, IOrderQueries orderQueries)
        {
            _orderService = orderService;
            _orderQueries = orderQueries;
        }

        /// <summary>
        /// Lấy danh sách tất cả đơn hàng
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderQueries.GetAllAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Lấy chi tiết 1 đơn hàng theo Id
        /// </summary>
        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderQueries.GetByIdAsync(orderId);
            if (order == null) return NotFound("Không tìm thấy đơn hàng.");
            return Ok(order);
        }

        /// <summary>
        /// Lấy danh sách đơn hàng theo User (lịch sử mua hàng)
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        public async Task<IActionResult> GetOrdersByUser(Guid userId)
        {
            var orders = await _orderQueries.GetByUserAsync(userId);
            if (orders == null || !orders.Any())
                return NotFound("Người dùng này chưa có đơn hàng nào.");
            return Ok(orders);
        }

        /// <summary>
        /// Tạo đơn hàng mới
        /// </summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request?.Order?.ProductIds == null || !request.Order.ProductIds.Any())
            {
                return BadRequest("Giỏ hàng trống hoặc dữ liệu không hợp lệ.");
            }

            var orderId = await _orderService.CreateOrderAsync(request.Order, request.Bill);

            return CreatedAtAction(nameof(GetOrderById), new { orderId }, new
            {
                Message = "Tạo đơn hàng thành công",
                OrderId = orderId
            });
        }

        /// <summary>
        /// Duyệt đơn hàng (Admin)
        /// </summary>
        [HttpPut("{orderId}/approve")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ApproveOrder(Guid orderId)
        {
            var success = await _orderService.ApproveOrderAsync(orderId);
            if (!success) return BadRequest("Chỉ có thể duyệt đơn hàng ở trạng thái Created.");
            return Ok(new { Message = "Đơn hàng đã được duyệt" });
        }

        /// <summary>
        /// Từ chối đơn hàng (Admin)
        /// </summary>
        [HttpPut("{orderId}/reject")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RejectOrder(Guid orderId)
        {
            var success = await _orderService.RejectOrderAsync(orderId);
            if (!success) return BadRequest("Chỉ có thể từ chối đơn hàng ở trạng thái Created hoặc SellerConfirmed.");
            return Ok(new { Message = "Đơn hàng đã bị từ chối" });
        }

        /// <summary>
        /// Hủy đơn hàng (User) - chỉ khi admin chưa duyệt
        /// </summary>
        [HttpPut("{orderId}/cancel")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CancelOrder(Guid orderId)
        {
            var success = await _orderService.CancelOrderAsync(orderId);
            if (!success) return BadRequest("Chỉ có thể hủy đơn hàng khi ở trạng thái Created.");
            return Ok(new { Message = "Đơn hàng đã bị hủy" });
        }
    }
}
