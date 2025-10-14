using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class FeaturedProductDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int? SalePercent { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public bool IsFeatured { get; set; }
        public string FeaturedType { get; set; }
    }
}