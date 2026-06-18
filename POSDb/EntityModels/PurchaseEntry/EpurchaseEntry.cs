using POSDb.EntityModels.MithaiShop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.PurchaseEntry
{
    public class EpurchaseEntry 
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public string PurchaseNo { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public int WarehouseId { get; set; }

        public string? Remarks { get; set; }

        // Navigation (Master → Details)
        public virtual ICollection<EPurchaseDetail> PurchaseDetails { get; set; }
            = new List<EPurchaseDetail>();
    }
    public class EPurchaseDetail
    {
        [Key]
        public int Id { get; set; }
        
        public int PurchaseId { get; set; }

        public int ProductId { get; set; }

        public int UnitId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GSTPercent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GSTAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        // Navigation (Child → Master)
        public virtual EpurchaseEntry? Purchase { get; set; }
    }
}
