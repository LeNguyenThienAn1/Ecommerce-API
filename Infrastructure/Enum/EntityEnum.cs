using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public enum ProductCategory
    {
        None = 0,
        TV = 1,
        Phone = 2,
        Laptop = 3,
        Tablet = 4,
        Accessory = 5,
        Headphone = 6,
        Camera = 7,
        SmartWatch = 8,
    }

    public enum ProductBrand
    {
        None = 0,
        Samsung = 1,
        Apple = 2,
        LG = 3,
        Sony = 4,
        Xiaomi = 5,
        Asus = 6,
        Acer = 7,
        Dell = 8,
        HP = 9,
        Huawei = 10,
        Oppo = 11,
        Vivo = 12,
    }
    public enum UserType
    {
        Admin = 0,
        Customer = 1,
    }
    public enum OrderStatus
    {
        Created = 0,
        SellerConfirmed = 1,
        PrepareShipping = 2,
        Rejected = 3,
        FailedShipping = 4,
        Successfully = 5,
        Paid = 6,
    }
    public enum CartStatus
    {
        CustomerVerified = 0,
        ConfirmPayment = 1,
        Finished = 2,
    }
    public enum ProductFeaturedType
    {
        Normal = 0,
        BestSeller = 1,
        New = 2,
        Popular = 3,
        Sale = 4,
    }
    public enum Color
    {
        None = 0,
        Blue = 1,
        Green = 2,
        Black = 3,
        White = 4,
        Yellow = 5,
        Pink = 6,
        Purple = 7,
        Orange = 8,
        Gray = 9,
        Brown = 10,
        Red = 11,
    }

    public enum Size
    {
        None,        // trường hợp mặc định
        Inch32,      // map "32-inch"
        Inch43,      // map "43-inch"
        Inch50,      // map "50-inch"
        Inch55,      // map "55-inch"
        Inch65,      // map "65-inch"
        Inch75,      // map "75-inch"
        Inch85       // map "85-inch"
    }
    public enum Capacity
    {
        None = 0,
        GB16 = 16,
        GB32 = 32,
        GB64 = 64,
        GB128 = 128,
        GB256 = 256,
        GB512 = 512,
        TB1 = 1024,
        TB2 = 2048,
    }
    public enum ProductStatus
    {
        Available = 0,
        Sold = 1
    }
}

