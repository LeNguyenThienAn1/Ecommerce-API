using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entity
{
    public class BrandEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }

        public ICollection<ProductEntity>? Products { get; set; }
    }
}
