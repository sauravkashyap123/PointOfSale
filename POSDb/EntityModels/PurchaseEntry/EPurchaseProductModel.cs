using POSDb.EntityModels.MithaiShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.PurchaseEntry
{
    public class EPurchaseProductModel:EBaseModel
    {
        public string? PurchaseProductCode { get; set; }
        public string? ProductName { get; set; }
        public int CategoryId { get; set; }
        public int Warehouseid { get; set; }
    }
}
