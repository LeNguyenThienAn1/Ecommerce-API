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
        /// <param name="keyword">Từ khóa cần tìm (ví dụ: "áo thun", "giày").</param>
        /// <returns>Danh sách ProductDto phù hợp.</returns>
        Task<List<ProductDto>> SearchProductsAsync(string keyword);
    }
}
