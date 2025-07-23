using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Queries
{
    public interface IProductQueries
    {
        public Task<ProductEntity> GetProductAsync();
    }
}
