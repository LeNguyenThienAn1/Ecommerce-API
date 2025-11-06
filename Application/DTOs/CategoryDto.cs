using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
        public class CategoryDto : BaseDto
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
        public class CreateOrUpdateCategoryDto : CommonPayloadDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        public Guid? CreateBy { get; set; }
        public Guid? UpdateBy { get; set; }

    }
    public class CategoryFilterDto : CommonPayloadDto
        {
            public Guid? CategoryId { get; set; } // Id của Category
            public string CategoryName { get; set; }

        }
        public class CategoryInfoDto
        {
            public Guid? CategoryId { get; set; } // Id của Category
            public string CategoryName { get; set; }
            public string Description { get; set; }
    }
    }

