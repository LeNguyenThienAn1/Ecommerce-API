using System.Collections.Generic;

namespace Application.DTOs
{
    /// <summary>
    /// DTO phản hồi từ chatbot sau khi xử lý yêu cầu của người dùng.
    /// </summary>
    public class ChatResponseDto
    {
        /// <summary>
        /// Tin nhắn trả lời của chatbot (nội dung text).
        /// </summary>
        public string BotMessage { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách sản phẩm (nếu chatbot tìm thấy kết quả từ database).
        /// Nếu không có sản phẩm, trả về danh sách rỗng.
        /// </summary>
        public List<ProductDto> Products { get; set; } = new();
    }
}
