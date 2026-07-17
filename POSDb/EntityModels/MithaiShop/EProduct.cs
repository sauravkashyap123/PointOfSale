using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.MithaiShop
{
    public class EProduct:EBaseModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public int CategoryId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchaseRate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaleRate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GSTPercent { get; set; }

        public int UnitId { get; set; }

        public bool IsWeightMachine { get; set; } = false;
        public bool IsPieceWise { get; set; } = false;

        public int ExpiryDays { get; set; }

        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}