using Model.Enum;

namespace Entity;

public class ProductEntity : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ProductBrand Brand { get; set; }
    public ProductCategory Category { get; set; } // e.g., TV, Phone, Laptop
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; }
    public bool IsFeatured { get; set; } = true;
    public bool IsOnSale { get; set; }  = false;

}
