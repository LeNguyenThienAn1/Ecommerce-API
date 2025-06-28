namespace Ecommerce_API.Model.Entity
{
    public class AddressEntity : CommonEntity
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
