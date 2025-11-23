using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entity
{
    public class ProductCommentEntity : BaseEntity
    {
        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductEntity Product { get; set; }

        public bool IsDelete { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }

        public string Content { get; set; }
    }
}
