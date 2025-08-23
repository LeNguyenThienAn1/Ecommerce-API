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
        Task<IEnumerable<ProductEntity>> GetAllProductsAsync();

        // Lấy sản phẩm theo ID
        Task<ProductEntity> GetProductByIdAsync(Guid id);

        // Cập nhật sản phẩm
        Task<bool> UpdateProductAsync(ProductEntity productEntity);

        // Tạo mới sản phẩm
        Task<bool> CreateProductAsync(ProductEntity entity);

        // Xóa sản phẩm
        Task<bool> DeleteProductAsync(Guid id);
    }
}
