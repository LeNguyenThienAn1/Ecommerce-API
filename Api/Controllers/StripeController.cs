using Application.DTOs;
using Application.EntityHandler.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly IStripeService _stripeService;
        private readonly IOrderService _orderService;

        public StripeController(IStripeService stripeService, IOrderService orderService)
        {
            _stripeService = stripeService;
            _orderService = orderService;
        }

        /// <summary>
        /// ✅ Tạo phiên thanh toán Stripe Checkout
        /// </summary>
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateSession([FromBody] StripePaymentDto dto)
        {
            if (dto == null)
                return BadRequest("Thiếu dữ liệu thanh toán.");

            try
            {
                var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(dto);
                if (string.IsNullOrWhiteSpace(checkoutUrl))
                    return BadRequest("Không thể tạo phiên thanh toán Stripe.");

                // ⚠️ FE mong đợi key là "checkoutUrl"
                return Ok(new { checkoutUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [StripeController Error] {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// ✅ Xác nhận thanh toán Stripe thành công
        /// </summary>
        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm(
            [FromQuery] string sessionId,
            [FromQuery] Guid orderId,
            [FromQuery] Guid userId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return BadRequest("Thiếu sessionId.");

            try
            {
                var success = await _stripeService.ConfirmPaymentAsync(sessionId);
                if (!success)
                    return BadRequest("Thanh toán Stripe thất bại hoặc chưa hoàn tất.");

                await _orderService.ConfirmPaymentSuccessAsync(orderId, userId);

                return Ok(new { message = "Thanh toán Stripe thành công!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [Stripe Confirm Error] {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
