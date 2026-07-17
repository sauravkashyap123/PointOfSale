using POSDb.EntityModels.POSModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels
{
    public class EShop
    {
        [Key]
        public int Id { get; set; }

        public string ShopName { get; set; }
        public string? OwnerName { get; set; }
        public string? ContactNumber { get; set; }

        // ✅ Foreign Key
        public int WarehouseId { get; set; }
        public bool IActive { get; set; } = true;

        // ✅ Navigation Property
        public EWarehouseModel Warehouse { get; set; }
    }
}
