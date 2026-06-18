using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class ShopModel
    {
        public int Id { get; set; }

        public string ShopName { get; set; }
        public string? OwnerName { get; set; }
        public string? ContactNumber { get; set; }
        public string? WarehouseName { get; set; }

        public List<SelectListItem>? selectWarehouse { get; set; }
        // ✅ Foreign Key
        public int WarehouseId { get; set; }
    }
}
