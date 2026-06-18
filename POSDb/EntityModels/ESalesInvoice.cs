using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels
{
    public class ESalesInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }

        public int shopid { get; set;  }
        public int staffid { get; set;  }
        public string? CustomerName { get; set; }
        public string? MobileNo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossAmount { get; set; } = 0.00m;
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercent { get; set; } = 0.00m;
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0.00m;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } = 0.00m;

        public string? PaymentMethod { get; set; }
        public DateTime InvoiceDate { get; set; }

    }
    public class ESalesInvoiceDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }

        public int ProductId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
    }
}
