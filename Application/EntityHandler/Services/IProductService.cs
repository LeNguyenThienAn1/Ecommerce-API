using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(Guid id);
        Task<List<ProductInfoDto>> GetAllProductAsync(ProductSearchDto searchDto);
        Task<bool> CreateOrUpdateProductAsync(CreateOrUpdateProductDto dto);
        Task<bool> DeleteProductAsync(Guid id);
    }
}
