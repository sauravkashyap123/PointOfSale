using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class ShopSaleListVM
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string ShopName { get; set; }
        public decimal GrossAmount { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string CategoryName { get; set; }
        public string ShortName { get; set; }
    }
}
