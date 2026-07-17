using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.MithaiShop
{
    public class EStock
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        public virtual ICollection<EStockHistory> StockHistories { get; set; }
            = new List<EStockHistory>();
    }

    public class EStockHistory
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(EStock))]
        public int StockId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        public string Type { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime? UpdateDate { get; set; }
        public string? Remark { get; set; }

        public virtual EStock EStock { get; set; }
    }
}
