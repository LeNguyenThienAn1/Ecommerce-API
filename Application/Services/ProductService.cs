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
            try
            {
                var products = await _productQueries.GetAllProductsAsync()
                        ?? Enumerable.Empty<ProductEntity>();

                var productsDict = products.GroupBy(p => p.Category).ToDictionary(g => g.Key, g => g.Select(p => MapToDto(p)));
                var result = new List<ProductDto>();
                foreach (var product in productsDict.Keys)
                {
                    var productDto = productsDict[product].First();
                    productDto.Stock = productsDict[product].Count();
                    result.Add(productDto);
                }
                return result;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                throw new ApplicationException("Đã xảy ra lỗi khi lấy tất cả sản phẩm.", ex);
            }
            
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
            if (dto.Id == Guid.Empty) // CREATE
            {
                var productEntitiesToAdd = new List<ProductEntity>();
                
                for (int index = 0; index < dto.Stock; index++)
                {
                    var productEntity = new ProductEntity
                    {
                        Name = dto.Name,
                        Description = dto.Description,
                        Price = dto.Price,
                        ImageUrl = dto.ImageUrl,
                        IsFeatured = dto.IsFeatured,
                        FeaturedType = dto.FeaturedType,
                        SalePercent = dto.SalePercent,
                        //    CategoryId = dto.CategoryId,
                        CreateAt = DateTime.UtcNow,
                        UpdateAt = DateTime.UtcNow,
                        CreateBy = "admin",
                        UpdateBy = "admin",
                        Detail = dto.Detail,
                        Brand = dto.Brand,
                        Category = dto.Category,
                    };
                    productEntitiesToAdd.Add(productEntity);
                }    
                

                return await _productQueries.CreateProductAsync(productEntitiesToAdd);
            }
            else // UPDATE
            {
                var existing = await _productQueries.GetProductByIdAsync(dto.Id);
                if (existing == null) return false;

                // update fields trực tiếp trên entity đã được track
                existing.Name = dto.Name;
                existing.Description = dto.Description;
                existing.Price = dto.Price;
                existing.ImageUrl = dto.ImageUrl;
                existing.IsFeatured = dto.IsFeatured;
                existing.FeaturedType = dto.FeaturedType;
                existing.SalePercent = dto.SalePercent;
              //  existing.CategoryId = dto.CategoryId;
                existing.UpdateAt = DateTime.UtcNow;
                existing.UpdateBy = "admin";
                existing.Detail = dto.Detail;

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
                IsFeatured = entity.IsFeatured,
                FeaturedType = entity.FeaturedType,
                SalePercent = entity.SalePercent,
                Brand = entity.Brand,
                Category = entity.Category,
                Detail = entity.Detail,
                //CategoryId = entity.CategoryId
            };
        }
    }
}
