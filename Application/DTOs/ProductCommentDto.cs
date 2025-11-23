namespace Application.DTOs
{
    public class ProductCommentDto : BaseDto
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public UserDto User { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
