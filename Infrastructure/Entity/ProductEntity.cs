namespace Infrastructure;

public class ProductEntity : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int Stock { get; set; }
    public bool IsFeatured { get; set; } = false;
    public ProductFeaturedType FeaturedType { get; set; } = ProductFeaturedType.Normal;
    public int? SalePercent { get; set; }
    public Guid CategoryId { get; set; }
}
