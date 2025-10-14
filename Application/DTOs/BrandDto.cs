using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class BrandDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }

    public class BrandInfoDto
    {
        public Guid BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }

    public class BrandFilterDto
    {
        public string? Keyword { get; set; }
    }

    public class CreateOrUpdateBrandDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }
}
