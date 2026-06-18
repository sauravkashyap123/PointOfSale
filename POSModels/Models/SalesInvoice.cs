using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class SalesInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }

        public string CustomerName { get; set; }
        public string MobileNo { get; set; }

        public decimal GrossAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
    public class SalesInvoiceDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }

        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
