using Infrastructure.Entity;
using System.Collections.Generic;

namespace Infrastructure;

public class ProductEntity : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    //public int Stock { get; set; }
    public bool IsFeatured { get; set; } = false;
    public ProductFeaturedType FeaturedType { get; set; } = ProductFeaturedType.Normal;
    public int? SalePercent { get; set; }
    //  public Guid CategoryId { get; set; }
    public Guid BrandId { get; set; }
    public BrandEntity Brand { get; set; }
    public Guid CategoryId { get; set; }
    public CategoryEntity Category { get; set; }
    public ProductDetail Detail { get; set; }
    public ProductStatus Status { get; set; }
    public Guid BoughtBy { get; set; } = Guid.Empty; // nếu khác Guid.Empty thì sản phẩm đã bán

    // Navigation property
    public ICollection<WishlistEntity> Wishlists { get; set; } = new List<WishlistEntity>();
    public ICollection<ProductCommentEntity> ProductComments { get; set; } = new List<ProductCommentEntity>();
}
public class ProductDetail
{
    public string Size { get; set; }
    public Color Color { get; set; }
    public Capacity Capacity { get; set; }
    public int BatteryCapacity { get; set; }

}
