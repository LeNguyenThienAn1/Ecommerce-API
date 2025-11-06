using Infrastructure;
using Infrastructure.Entity;
using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    /// <summary>
    /// DTO dùng để trả dữ liệu sản phẩm ra FE
    /// </summary>
    public class ProductDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; } = 1;
        public bool IsFeatured { get; set; }
        public ProductFeaturedType FeaturedType { get; set; }
        public int? SalePercent { get; set; }

        // Khóa ngoại
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }

        // Navigation (dùng khi trả về chi tiết)
        public CategoryEntity Category { get; set; }
        public BrandEntity Brand { get; set; }

        // Thông tin chi tiết sản phẩm
        public ProductDetail Detail { get; set; }
    }

    /// <summary>
    /// DTO dùng để nhận dữ liệu từ FE khi tạo hoặc cập nhật sản phẩm
    /// </summary>
    public class CreateOrUpdateProductDto : CommonPayloadDto
    {
        public Guid Id { get; set; }  // khi tạo mới => Guid.Empty, khi update => truyền Id
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; } = 1;
        public bool IsFeatured { get; set; }
        public ProductFeaturedType FeaturedType { get; set; }
        public int? SalePercent { get; set; }

        // Chỉ cần gửi ID, backend sẽ map sang entity
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }

        // Tùy nhu cầu FE: nếu FE cho phép nhập detail trực tiếp => giữ lại
        public ProductDetail Detail { get; set; }
    }

    /// <summary>
    /// Dùng cho lọc hoặc tìm kiếm sản phẩm
    /// </summary>
    public class ProductSearchDto
    {
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
    }

    /// <summary>
    /// Dùng cho autocomplete hoặc dropdown
    /// </summary>
    public class ProductInfoDto
    {
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
    }

    /// <summary>
    /// Dùng cho phân trang + lọc (User/Admin)
    /// </summary>
    public class ProductPagingRequestDto : PagedAndFilterDto
    {
        // FE gửi danh sách ID cần lọc
        public List<Guid>? BrandIds { get; set; } = new();
        public List<Guid>? CategoryIds { get; set; } = new();
    }

    /// <summary>
    /// Kết quả trả về có phân trang
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
