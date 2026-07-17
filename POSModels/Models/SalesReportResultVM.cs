using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class SalesReportResultVM
    {
        public SalesReportSummaryVM Summary { get; set; }
        public List<SalesReportRowVM> Rows { get; set; }
    }

    public class SalesReportSummaryVM
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AvgOrderValue { get; set; }
        public int ShopCount { get; set; }
    }

    // ---- One row per shop for the selected period ----
    public class SalesReportRowVM
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AvgOrderValue { get; set; }

        // Shown in table as a pill (e.g. "01 Jul - 07 Jul 2026", "July 2026", "2026")
        public string PeriodLabel { get; set; }

        // Machine-readable key sent back when user clicks "View" to fetch day-wise detail
        // Week  -> "2026-W27"
        // Month -> "2026-07"
        // Year  -> "2026"
        public string PeriodKey { get; set; }
    }

    // ---- Day-wise breakup shown inside the expandable detail panel ----
    public class SalesReportDetailRowVM
    {
        public DateTime SaleDate { get; set; }
        public int InvoiceCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
