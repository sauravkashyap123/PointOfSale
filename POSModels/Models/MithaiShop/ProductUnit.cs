using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.MithaiShop
{
    public class ProductUnit:BaseModel
    {
        public int ProductId { get; set; }

        public string UnitName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaleRate { get; set; }

        public string Barcode { get; set; }

    }
}
