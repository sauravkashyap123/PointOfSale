using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class InvoicePrintModel
    {
        public int Id { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string CustomerName { get; set; }

        public string MobileNo { get; set; }

        public string PaymentMethod { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal DiscountPercent { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public List<InvoiceItemModel> Items { get; set; } = new();

        public string? ShopName { get; set; }
        public string? GstNumber { get; set; }
        public string? PrintMobileno { get; set; }
        public string? PrintAddress { get; set; }
    }

    public class InvoiceItemModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string UnitName { get; set; }

        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }

        public decimal Amount { get; set; }
    }
}
