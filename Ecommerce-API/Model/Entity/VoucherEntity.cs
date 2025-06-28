namespace Ecommerce_API.Model.Entity
{
    public class VoucherEntity : CommonEntity
    {
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;      
    }
}
