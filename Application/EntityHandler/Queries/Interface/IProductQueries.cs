using Application.DTOs;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Queries
{
    public interface IProductQueries
    {
        // Lấy tất cả sản phẩm
        Task<PagedResult<ProductDto>> GetAllProductsAsync(ProductPagingRequestDto request);

        // Lấy sản phẩm theo ID
        Task<ProductEntity> GetProductByIdAsync(Guid id);

        // Cập nhật sản phẩm
        Task<bool> UpdateProductAsync(ProductEntity entity);

        // Tạo mới sản phẩm
        Task<bool> CreateProductAsync(List<ProductEntity> productEntities);

        // Xóa sản phẩm
        Task<bool> DeleteProductAsync(Guid id);
        Task<List<BrandDto>> GetAllBrandAsync();
    }
}
