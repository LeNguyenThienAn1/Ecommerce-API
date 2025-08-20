using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class CategoryEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Quan hệ: 1 Category có nhiều Product
        public ICollection<ProductEntity> Products { get; set; }
    }
}
