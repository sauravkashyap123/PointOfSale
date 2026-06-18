using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.MithaiShop
{
    public class EProduction:EBaseModel
    {
        public string ProductionNo { get; set; }

        public DateTime ProductionDate { get; set; }

        public int ProductId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostAmount { get; set; }
        public int UsedQuantity { get; set; }
        public int SaleQuantity { get; set; }
        public int TransferQuantity { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string? Remarks { get; set; }
    }
}
