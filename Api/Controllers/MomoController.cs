// Api/Controllers/MomoController.cs

using Application.DTOs;
using Application.EntityHandler.Services;
using Application.Interfaces;
using Infrastructure; // Giả định OrderStatus được định nghĩa hoặc có thể truy cập từ đây
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MomoController : ControllerBase
    {
        private readonly IMomoService _momoService;
        private readonly IOrderService _orderService;

        public MomoController(IMomoService momoService, IOrderService orderService)
        {
            _momoService = momoService;
            _orderService = orderService;
        }

        /// <summary>
        /// Tạo thanh toán MoMo với orderId thật từ DB
        /// ✅ Frontend gọi endpoint này sau khi tạo Order thành công
        /// </summary>
        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] JsonElement body)
        {
            // ✅ Nhận orderId, amount, orderInfo từ frontend
            if (!body.TryGetProperty("orderId", out var orderIdProp) ||
                !body.TryGetProperty("amount", out var amountProp) ||
                !body.TryGetProperty("orderInfo", out var orderInfoProp))
            {
                return BadRequest(new { message = "Thiếu orderId, amount hoặc orderInfo" });
            }

            // Parse orderId
            var orderIdString = orderIdProp.GetString();
            if (string.IsNullOrEmpty(orderIdString) || !Guid.TryParse(orderIdString, out var orderId))
                return BadRequest(new { message = "orderId không hợp lệ" });

            // Parse amount
            var amountString = amountProp.GetString();
            if (string.IsNullOrEmpty(amountString) || !decimal.TryParse(amountString, out var amount))
                return BadRequest(new { message = "amount không hợp lệ" });

            var orderInfo = orderInfoProp.GetString() ?? "Thanh toán MoMo";

            // ✅ Lấy Order thật từ DB
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng" });

            // ✅ Kiểm tra trạng thái Order (chỉ cho phép Created)
            // Cần truy cập OrderStatus.Created
            // if (order.Status != OrderStatus.Created) 
            //     return BadRequest(new { message = $"Đơn hàng đang ở trạng thái '{order.Status}', không thể thanh toán" });

            // ✅ Gọi MomoService để tạo payment request
            var momoResponse = await _momoService.CreatePaymentAsync(order, amount);

            if (momoResponse == null)
                return StatusCode(500, new { message = "Không thể khởi tạo thanh toán MoMo" });

            // ✅ Trả về payUrl để frontend redirect
            return Ok(momoResponse);
        }

        /// <summary>
        /// Nhận callback (IPN) từ MoMo khi thanh toán xong
        /// ✅ MoMo gọi endpoint này để thông báo kết quả thanh toán
        /// </summary>
        [HttpPost("notify")]
        public async Task<IActionResult> Notify([FromBody] MomoIPNResponseDto ipn)
        {
            if (ipn == null)
                return BadRequest(new { message = "Dữ liệu IPN trống" });

            // ✅ Validate chữ ký từ MoMo
            var valid = await _momoService.ValidateSignatureAsync(ipn);
            if (!valid)
            {
                Console.WriteLine($"[MoMo IPN] Invalid signature for OrderId: {ipn.OrderId}");
                return BadRequest(new { message = "Chữ ký không hợp lệ" });
            }

            Console.WriteLine($"[MoMo IPN] OrderId: {ipn.OrderId}, ResultCode: {ipn.ResultCode}, TransId: {ipn.TransId}");

            // ✅ Nếu thanh toán thành công (resultCode = 0)
            if (ipn.ResultCode == 0)
            {
                if (!Guid.TryParse(ipn.OrderId, out var orderId))
                {
                    Console.WriteLine($"[MoMo IPN] Invalid OrderId format: {ipn.OrderId}");
                    return BadRequest(new { message = "OrderId không hợp lệ" });
                }

                // Cập nhật trạng thái Order thành công (Trừ sản phẩm)
                var updated = await _orderService.ConfirmPaymentSuccessAsync(orderId);

                if (updated)
                {
                    Console.WriteLine($"[MoMo IPN] ✅ Order {orderId} confirmed successfully");
                }
                else
                {
                    Console.WriteLine($"[MoMo IPN] ⚠️ Failed to confirm Order {orderId} (not found or invalid status)");
                }
            }
            else
            {
                Console.WriteLine($"[MoMo IPN] ❌ Payment failed for OrderId: {ipn.OrderId}, Message: {ipn.Message}");

                // (Tùy chọn) Xử lý đơn hàng thất bại
                // if (Guid.TryParse(ipn.OrderId, out var orderId)) {
                //    await _orderService.CancelOrderAsync(orderId);
                // }
            }

            // ✅ Luôn trả về 200 OK cho MoMo (theo yêu cầu của MoMo API)
            return Ok(new { message = "IPN received", resultCode = 0 });
        }

        // --- ENDPOINT MỚI ---

        /// <summary>
        /// Endpoint được Frontend gọi để xác nhận thanh toán thành công sau khi MoMo Redirect.
        /// 💡 Đây là giải pháp thay thế cho IPN khi chạy trên localhost.
        /// </summary>
        /// <param name="dto">Chứa OrderId và ResultCode từ query string của trình duyệt.</param>
        [HttpPost("confirm-frontend")]
        public async Task<IActionResult> ConfirmPaymentFromFrontend([FromBody] MomoFrontendConfirmDto dto)
        {
            // 1. Kiểm tra mã thành công
            if (dto.ResultCode != 0)
            {
                return BadRequest(new { message = $"Thanh toán thất bại. Mã lỗi: {dto.ResultCode}" });
            }

            // 2. Parse OrderId
            if (string.IsNullOrEmpty(dto.OrderId) || !Guid.TryParse(dto.OrderId, out var orderId))
            {
                return BadRequest(new { message = "OrderId không hợp lệ" });
            }

            // 3. Gọi OrderService để cập nhật trạng thái đơn hàng (trừ sản phẩm)
            var updated = await _orderService.ConfirmPaymentSuccessAsync(orderId);

            if (updated)
            {
                Console.WriteLine($"[FE Confirm] ✅ Order {orderId} confirmed successfully by Frontend call.");
                return Ok(new { orderId = orderId, message = "Xác nhận thanh toán thành công từ Frontend" });
            }
            else
            {
                // Trả về lỗi nếu đơn hàng đã được cập nhật hoặc không tìm thấy
                return BadRequest(new { message = "Không thể cập nhật trạng thái đơn hàng (đã được cập nhật hoặc không hợp lệ)." });
            }
        }

        // --- ENDPOINT XỬ LÝ REDIRECT GIỮ NGUYÊN ---

        /// <summary>
        /// Endpoint xử lý redirect từ MoMo (returnUrl)
        /// ✅ User được MoMo redirect về đây sau khi thanh toán
        /// </summary>
        [HttpGet("return")]
        public IActionResult Return(
            [FromQuery] string partnerCode,
            [FromQuery] string orderId,
            [FromQuery] string requestId,
            [FromQuery] string amount,
            [FromQuery] string orderInfo,
            [FromQuery] string orderType,
            [FromQuery] string transId,
            [FromQuery] string resultCode,
            [FromQuery] string message,
            [FromQuery] string payType,
            [FromQuery] string responseTime,
            [FromQuery] string extraData,
            [FromQuery] string signature)
        {
            Console.WriteLine($"[MoMo Return] OrderId: {orderId}, ResultCode: {resultCode}");

            // ✅ Redirect về frontend với query params
            // Frontend (React) sẽ bắt resultCode và gọi API confirm-frontend
            var frontendUrl = $"https://localhost:5173/cart?resultCode={resultCode}&orderId={orderId}&message={message}";

            return Redirect(frontendUrl);
        }
    }
}