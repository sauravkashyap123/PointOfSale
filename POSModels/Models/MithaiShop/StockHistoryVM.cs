using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.MithaiShop
{
    public class StockHistoryVM
    {
        public decimal Quantity { get; set; }
        public string Type { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public DateTime EntryDate { get; set; }
        public string CategoryName { get; set; }
    }
}
