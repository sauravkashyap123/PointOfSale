using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POSDb.EntityModels.MithaiShop
{
    public class EBulkOrder : EBaseModel
    {
        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; }

        [Required]
        [MaxLength(150)]
        public string CustomerName { get; set; }

        [Required]
        [MaxLength(20)]
        public string ContactNumber { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DeliveryDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Ready, Delivered, Cancelled

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdvanceAmount { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        public int ShopId { get; set; }

        public int StaffId { get; set; }

        public string? Remarks { get; set; }

        public virtual ICollection<EBulkOrderDetail> Items { get; set; } = new List<EBulkOrderDetail>();
    }
}
