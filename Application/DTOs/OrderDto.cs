using Infrastructure;

namespace Application.DTOs
{
    public class OrderDto : BaseDto
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string Note { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public OrderStatus Status { get; set; }

        public Guid UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    // ✅ Bổ sung các DTO cho MoMo
    public class MomoPaymentRequestDto
    {
        public string PartnerCode { get; set; }
        public string AccessKey { get; set; }
        public string RequestId { get; set; }
        public string Amount { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string ReturnUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string ExtraData { get; set; } = "";
        public string RequestType { get; set; } = "captureMoMoWallet";
        public string Signature { get; set; }
    }

    public class MomoPaymentResponseDto
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string LocalMessage { get; set; }
        public string RequestId { get; set; }
        public string OrderId { get; set; }
        public string PayUrl { get; set; }
        public string Deeplink { get; set; }
        public string QrCodeUrl { get; set; }
        public string Signature { get; set; }
    }

    public class MomoIPNResponseDto : CommonPayloadDto
    {
        public string PartnerCode { get; set; }
        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public long Amount { get; set; }
        public string OrderInfo { get; set; }
        public string OrderType { get; set; }
        public long TransId { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public string PayType { get; set; }
        public long ResponseTime { get; set; }
        public string ExtraData { get; set; }
        public string Signature { get; set; }
    }
    public class MomoFrontendConfirmDto : CommonPayloadDto
    {
        // Chứa OrderId nhận được từ URL MoMo redirect
        public string OrderId { get; set; }

        // Chứa resultCode (ví dụ: 0) nhận được từ URL MoMo redirect
        public int ResultCode { get; set; }
    }
    public class StripePaymentDto
    {
        public Guid OrderId { get; set; }
        public string Currency { get; set; } = "usd"; // đơn vị tiền
        public long Amount { get; set; }              // 1000 = $10.00
        public string SuccessUrl { get; set; }        // frontend redirect sau khi thanh toán thành công
        public string CancelUrl { get; set; }         // frontend redirect nếu hủy
    }

}
