using Application.DTOs;
using Application.Interfaces.Queries;
using Application.Interfaces.Services;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductQueries _productQueries;
        private object filter;

        public ProductService(IProductQueries productQueries)
        {
            _productQueries = productQueries;
        }

        /// <summary>
        /// Lấy tất cả sản phẩm với filter (dùng cho User)
        /// </summary>
        public async Task<List<ProductInfoDto>> GetAllProductsAsync(ProductSearchDto search)
        {
            var products = await _productQueries.GetAllProductsAsync()
                           ?? Enumerable.Empty<ProductEntity>();

            // Search theo tên sản phẩm
            if (!string.IsNullOrWhiteSpace(search?.ProductName))
            {
                var searchContent = search.ProductName.Trim().ToLower();
                products = products.Where(c => c.Name != null &&
                                               c.Name.ToLower().Contains(searchContent));
            }

            // Filter theo ProductId
            if (search?.ProductId is Guid productId && productId != Guid.Empty)
            {
                products = products.Where(c => c.Id == productId);
            }

            var result = new List<ProductInfoDto>();
            foreach (var product in products)
            {
                var productDto = new ProductInfoDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                };
                result.Add(productDto);
            }
            return result;
        }

        /// <summary>
        /// Lấy tất cả sản phẩm (không filter) - mapping sang ProductDto
        /// </summary>
        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productQueries.GetAllProductsAsync()
                           ?? Enumerable.Empty<ProductEntity>();

            return products.Select(p => MapToDto(p)).ToList();
        }

        /// <summary>
        /// Lấy tất cả sản phẩm có search filter (mapping sang ProductInfoDto)
        /// </summary>
        public async Task<List<ProductInfoDto>> GetAllProductAsync(ProductSearchDto searchDto)
        {
            return await GetAllProductsAsync(searchDto);
        }

        /// <summary>
        /// Lấy sản phẩm theo Id
        /// </summary>
        public async Task<ProductDto> GetProductByIdAsync(Guid id)
        {
            var entity = await _productQueries.GetProductByIdAsync(id);
            if (entity == null)
                return null;

            return MapToDto(entity);
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật sản phẩm
        /// </summary>
        public async Task<bool> CreateOrUpdateProductAsync(CreateOrUpdateProductDto dto)
        {
            var productEntity = new ProductEntity
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                Stock = dto.Stock,
                IsFeatured = dto.IsFeatured,
                FeaturedType = dto.FeaturedType,
                SalePercent = dto.SalePercent,
                CategoryId = dto.CategoryId
            };

            if (dto.Id == Guid.Empty) // create
            {
                return await _productQueries.CreateProductAsync(productEntity);
            }
            else // update
            {
                var existing = await _productQueries.GetProductByIdAsync(dto.Id);
                if (existing == null) return false;

                productEntity.Id = dto.Id;
                return await _productQueries.UpdateProductAsync(productEntity);
            }
        }

        /// <summary>
        /// Xóa sản phẩm theo Id
        /// </summary>
        public async Task<bool> DeleteProductAsync(Guid id)
        {
            return await _productQueries.DeleteProductAsync(id);
        }

        // ===== Private helper =====
        private ProductDto MapToDto(ProductEntity entity)
        {
            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                ImageUrl = entity.ImageUrl,
                Stock = entity.Stock,
                IsFeatured = entity.IsFeatured,
                FeaturedType = entity.FeaturedType,
                SalePercent = entity.SalePercent,
                CategoryId = entity.CategoryId
            };
        }
    }
}
