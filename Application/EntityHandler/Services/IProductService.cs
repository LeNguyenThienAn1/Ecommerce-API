using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IProductService
    {
        /// <summary>
        /// Lấy sản phẩm có phân trang
        /// </summary>
        Task<PagedResult<ProductDto>> GetPagedProductsAsync(ProductPagingRequestDto request);

        /// <summary>
        /// Lấy sản phẩm theo Id
        /// </summary>
        Task<ProductDto?> GetProductByIdAsync(Guid id);

        /// <summary>
        /// Lấy sản phẩm theo search (không phân trang)
        /// </summary>
        Task<List<ProductInfoDto>> GetAllProductsAsync(ProductSearchDto searchDto);

        /// <summary>
        /// Tạo mới hoặc cập nhật sản phẩm
        /// </summary>
        Task<bool> CreateOrUpdateProductAsync(CreateOrUpdateProductDto dto);

        /// <summary>
        /// Xóa sản phẩm theo Id
        /// </summary>
        Task<bool> DeleteProductAsync(Guid id);

        /// <summary>
        /// Lấy tất cả brand
        /// </summary>
        Task<List<BrandDto>> GetAllBrandsAsync();
    }
}
