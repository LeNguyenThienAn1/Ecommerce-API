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
}

