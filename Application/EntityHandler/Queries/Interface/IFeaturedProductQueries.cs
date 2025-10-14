using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EntityHandler.Queries.Interface
{
    public interface IFeaturedProductQueries
    {
        Task<IEnumerable<FeaturedProductDto>> GetFeaturedProductsAsync();
    }
}
