using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityHandler.Queries.Interface
{
    /// <summary>
    /// Interface định nghĩa các truy vấn liên quan đến chatbot và sản phẩm.
    /// </summary>
    public interface IChatQueries
    {
        /// <summary>
        /// Tìm kiếm danh sách sản phẩm theo từ khóa người dùng nhập vào.
        /// </summary>
        /// <param name="keyword">Từ khóa cần tìm.</param>
        /// <returns>Danh sách ProductDto phù hợp.</returns>
        Task<List<ProductDto>> SearchProductsAsync(string keyword);

        /// <summary>
        /// [NÂNG CẤP] Đếm tổng số lượng sản phẩm khớp với từ khóa tìm kiếm.
        /// Dùng để hỗ trợ logic kiểm tra tồn kho.
        /// </summary>
                /// <param name="keyword">Từ khóa cần đếm số lượng.</param>
                /// <returns>Tổng số sản phẩm tìm thấy.</returns>
                Task<int> GetProductCountAsync(string keyword);
        
                /// <summary>
                /// [NÂNG CẤP] Tìm kiếm sản phẩm nâng cao theo nhiều tiêu chí.
                /// </summary>
                /// <param name="keywords">Từ khóa tìm kiếm (có thể là null).</param>
                /// <param name="category">Tên danh mục (có thể là null).</param>
                /// <param name="brand">Tên thương hiệu (có thể là null).</param>
                /// <param name="minPrice">Giá tối thiểu (có thể là null).</param>
                /// <param name="maxPrice">Giá tối đa (có thể là null).</param>
                /// <param name="sortBy">Tiêu chí sắp xếp.</param>
                /// <returns>Danh sách ProductDto phù hợp.</returns>
                        Task<List<ProductDto>> FindProductsByCriteriaAsync(
                            string? keywords,
                            string? category,
                            string? brand,
                            decimal? minPrice,
                            decimal? maxPrice,
                            Infrastructure.ProductSortBy sortBy
                        );            }
        }
        