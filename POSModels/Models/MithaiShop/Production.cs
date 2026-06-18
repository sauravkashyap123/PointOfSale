using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.MithaiShop
{
    public class Production:BaseModel
    {
        public string ProductionNo { get; set; }

        public DateTime ProductionDate { get; set; } = DateTime.Now;

        public int ProductId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostAmount { get; set; }

        public DateTime ExpiryDate { get; set; } = DateTime.Now;

        public string? Remarks { get; set; }

        public List<SelectListItem>? SelectedproductList { get; set; }
        public List<Production>? productionList { get; set; }
    }
    public class ProductionList
    {
        public string ProductionNo { get; set; }

        public DateTime ProductionDate { get; set; } = DateTime.Now;

        public int ProductId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostAmount { get; set; }

        public int UsedQuantity { get; set; }
        public int SaleQuantity { get; set;  }
        public int TransferQuantity { get; set;  }

        public DateTime ExpiryDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string? Remarks { get; set; }
        public string? ProductName { get; internal set; }
        public string? CategoryName { get; internal set; }
        public string? Unit { get; internal set; }
    }
}
