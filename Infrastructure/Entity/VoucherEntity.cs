using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entity
{
    public class VoucherEntity : BaseEntity
    {
        public int ThresHold { get; set; }
        public int DiscountPercent { get; set; }
        public int DiscountMoney { get; set; }
    }
}
