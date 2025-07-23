using Entity;
using Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiService
{
    public interface IProductService
    {
        public Task<ProductDto> GetProductAsync();
    }
}
