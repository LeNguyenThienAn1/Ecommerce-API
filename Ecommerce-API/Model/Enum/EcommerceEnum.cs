namespace Ecommerce_API
{
    public class EcommerceEnum
    {
        public enum ProductCategory
        {
            None = 0,
            TV = 1,
            Laptop = 2,
            Phone = 3,
            Mouse = 4,
            Keyboard = 5,
            Headphone = 6,
            Ipad = 7,
        }
        public enum Brand
        {
            None = 0,
            Apple = 1,
            Samsung = 2,
            Dell = 3,
            HP = 4,
            Asus = 5,
            Sony = 6,
            LG = 7,
            Microsoft = 8,
            Lenovo = 9,
            Xiaomi = 10
        }
        public enum UserRole
        {
            None = 0,
            Admin = 1,
            User = 2,
        }
    }
}
