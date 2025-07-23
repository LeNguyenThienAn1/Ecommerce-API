using Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTOs
{
    public class ProductDto : BaseDto
    {
        public string Name { get; set; }
        public ProductBrand Brand { get; set; }
        public ProductCategory Category { get; set; }
        public decimal Price { get; set; }
    }
}
