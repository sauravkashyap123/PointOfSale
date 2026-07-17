using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POSDb.EntityModels.MithaiShop
{
    public class EBulkOrderDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(EBulkOrder))]
        public int BulkOrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public virtual EBulkOrder BulkOrder { get; set; }
    }
}
