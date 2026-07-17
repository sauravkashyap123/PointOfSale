using Microsoft.AspNetCore.Mvc.Rendering;
using POSModels.Models.MithaiShop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.PurchaseEntry
{
    public class RawPurchaseModel : BaseModel
    {
        public string PurchaseNo { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public int WarehouseId { get; set; }

        public string? Remarks { get; set; }
        public decimal? TotalAmount { get; set; }

        public List<SelectListItem>? selectedProductList { get; set; }
        public List<SelectListItem>? selectedunitlist { get; set; }

        public List<RawPurchaseDetailModel> Items { get; set; } = new();
        public int ItemCount { get;  set; }
    }
    public class RawPurchaseDetailModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        // 🔥 UNIT (FK)
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GSTPercent { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal => Quantity * Rate;
        [Column(TypeName = "decimal(18,2)")]
        public decimal GSTAmount => (SubTotal * GSTPercent) / 100;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount => SubTotal + GSTAmount;
    }
}
