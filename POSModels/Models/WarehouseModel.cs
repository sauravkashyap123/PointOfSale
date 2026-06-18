using POSDb.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class WarehouseModel
    {
        public int Id { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string Address { get; set; }

        private int _noOfShop = 5;

        public int NoOfShop
        {
            get =>_noOfShop;
            set => _noOfShop = value > 5 ? 5 : value;
        }
        public string? Description { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public List<Shop> Shops { get; set; }
        public string? Password { get;  set; }
    }
    public class Shop
    {
        public int Id { get; set; }

        public string ShopName { get; set; }
        public string? OwnerName { get; set; }
        public string? ContactNumber { get; set; }

        // ✅ Foreign Key
        public int WarehouseId { get; set; }

        // ✅ Navigation Property
        public WarehouseModel Warehouse { get; set; }

        public static implicit operator Shop?(EShop? v)
        {
            throw new NotImplementedException();
        }
    }
}
