using Microsoft.AspNetCore.Mvc.Rendering;
using POSModels.Models.MithaiShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.PurchaseEntry
{
    public class PurchaseProductModel:BaseModel
    {
        public string? PurchaseProductCode { get; set; }
        public string? ProductName { get; set; }
        public int CategoryId { get; set; }
        public int Warehouseid { get; set;  }
        public List<SelectListItem>? SelectedCategoryList { get; set; }
    }
    
}
