using POSDb.EntityModels.MithaiShop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels
{
    public class EWarehousestock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Quantity { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        
    }
    public class EWarehouseStockHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal QuantityChanged { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal PreviousQuantity { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal NewQuantity { get; set; }

        public int? ReferenceId { get; set; }

        [MaxLength(50)]
        public string? ReferenceType { get; set; }
        public int? RelatedShopId { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }
    }
}
