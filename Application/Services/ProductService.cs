using Application.DTOs;
using Application.Interfaces.Queries;
using Application.Interfaces.Services;
using Infrastructure;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductQueries _productQueries;

        public ProductService(IProductQueries productQueries)
        {
            _productQueries = productQueries;
        }

        /// <summary>
        /// Lấy tất cả brands
        /// </summary>
        public async Task<List<BrandDto>> GetAllBrandsAsync()
        {
            return await _productQueries.GetAllBrandAsync();
        }

        /// <summary>
        /// Lấy sản phẩm phân trang (cho User/Admin)
        /// </summary>
        public async Task<PagedResult<ProductDto>> GetPagedProductsAsync(ProductPagingRequestDto request)
        {
            return await _productQueries.GetAllProductsAsync(request);
        }

        /// <summary>
        /// Lấy sản phẩm theo search (trả về ProductInfoDto)
        /// </summary>
        public async Task<List<ProductInfoDto>> GetAllProductsAsync(ProductSearchDto search)
        {
            var request = new ProductPagingRequestDto
            {
                PageNumber = 1,
                PageSize = int.MaxValue, // lấy hết
                SearchTerm = search?.ProductName
            };

            var result = await _productQueries.GetAllProductsAsync(request);
            var products = result.Items;

            // Filter theo ProductId nếu có
            if (search?.ProductId is Guid productId && productId != Guid.Empty)
            {
                products = products.Where(c => c.Id == productId).ToList();
            }

            return products.Select(p => new ProductInfoDto
            {
                ProductId = p.Id,
                ProductName = p.Name
            }).ToList();
        }

        /// <summary>
        /// Lấy sản phẩm theo Id
        /// </summary>
        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var entity = await _productQueries.GetProductByIdAsync(id);
            if (entity == null) return null;

            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                ImageUrl = entity.ImageUrl,
                IsFeatured = entity.IsFeatured,
                FeaturedType = entity.FeaturedType,
                SalePercent = entity.SalePercent,
              //  BrandId = entity.BrandId,
             //   CategoryId = entity.CategoryId,
                Detail = entity.Detail,
                CreateAt = entity.CreateAt
            };
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật sản phẩm
        /// </summary>
        public async Task<bool> CreateOrUpdateProductAsync(CreateOrUpdateProductDto dto)
        {
            if (dto.Id == Guid.Empty) // CREATE
            {
                var productEntitiesToAdd = new List<ProductEntity>();

                for (int i = 0; i < dto.Stock; i++)
                {
                    productEntitiesToAdd.Add(new ProductEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = dto.Name,
                        Description = dto.Description,
                        Price = dto.Price,
                        ImageUrl = dto.ImageUrl,
                        IsFeatured = dto.IsFeatured,
                        FeaturedType = dto.FeaturedType,
                        SalePercent = dto.SalePercent,
                        Detail = dto.Detail,
                        CategoryId = dto.CategoryId,   // ✅ dùng ID
                        BrandId = dto.BrandId,         // ✅ dùng ID
                        CreateAt = DateTime.UtcNow,
                        UpdateAt = DateTime.UtcNow,
                        CreateBy = "admin",
                        UpdateBy = "admin",
                        Status = ProductStatus.Available
                    });
                }

                return await _productQueries.CreateProductAsync(productEntitiesToAdd);
            }
            else // UPDATE
            {
                var existing = await _productQueries.GetProductByIdAsync(dto.Id);
                if (existing == null) return false;

                existing.Name = dto.Name;
                existing.Description = dto.Description;
                existing.Price = dto.Price;
                existing.ImageUrl = dto.ImageUrl;
                existing.IsFeatured = dto.IsFeatured;
                existing.FeaturedType = dto.FeaturedType;
                existing.SalePercent = dto.SalePercent;
                existing.Detail = dto.Detail;
                existing.CategoryId = dto.CategoryId;   // ✅ dùng ID
                existing.BrandId = dto.BrandId;         // ✅ dùng ID
                existing.UpdateAt = DateTime.UtcNow;
                existing.UpdateBy = "admin";

                return await _productQueries.UpdateProductAsync(existing);
            }
        }

        /// <summary>
        /// Xóa sản phẩm theo Id
        /// </summary>
        public async Task<bool> DeleteProductAsync(Guid id)
        {
            return await _productQueries.DeleteProductAsync(id);
        }
    }
}
