using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entity
{
    public class ProductDetailEntity : BaseEntity
    {
        public string Name { get; set; }
        public ProductCategory Category { get; set; }
        public ProductBrand Brand { get; set; }
        public string ImageUrl { get; set; }
    }
}
