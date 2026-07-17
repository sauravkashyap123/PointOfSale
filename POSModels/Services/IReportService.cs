using Microsoft.EntityFrameworkCore;
using POSDb.Data;
using POSModels.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Services
{
    public interface IReportService
    {
        Task<SalesReportResultVM> GetSalesReport(int shopId, string reportType, DateTime date);
        Task<List<SalesReportDetailRowVM>> GetSalesReportDetail(string reportType, string periodKey, int shopId);
    }

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;   // TODO: apne actual DbContext class ka naam daalo

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================== MAIN REPORT (Week / Month / Year, shop wise) ==================
        public async Task<SalesReportResultVM> GetSalesReport(int shopId, string reportType, DateTime date)
        {
            DateTime startDate;
            DateTime endDate;
            string periodLabel;
            string periodKey;

            GetPeriodRange(reportType, date, out startDate, out endDate, out periodLabel, out periodKey);

            var query =
                from d in _context.tblSaleInvoice
                join s in _context.tblShop on d.shopid equals s.Id
                where d.InvoiceDate >= startDate && d.InvoiceDate < endDate
                select new { d, s };

            if (shopId > 0)
                query = query.Where(x => x.d.shopid == shopId);

            var grouped = await query
                .GroupBy(x => new { x.d.shopid, x.s.ShopName })
                .Select(g => new SalesReportRowVM
                {
                    ShopId = g.Key.shopid,
                    ShopName = g.Key.ShopName,
                    TotalOrders = g.Count(),
                    TotalSales = g.Sum(x => x.d.TotalAmount),
                    PeriodLabel = periodLabel,
                    PeriodKey = periodKey
                })
                .OrderBy(x => x.ShopName)
                .ToListAsync();

            // AvgOrderValue ko in-memory calculate karte hain (divide by zero se bachne ke liye)
            foreach (var row in grouped)
            {
                row.AvgOrderValue = row.TotalOrders > 0
                    ? Math.Round(row.TotalSales / row.TotalOrders, 2)
                    : 0;
            }

            var totalOrders = grouped.Sum(x => x.TotalOrders);
            var totalSales = grouped.Sum(x => x.TotalSales);

            var summary = new SalesReportSummaryVM
            {
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                ShopCount = grouped.Count,
                AvgOrderValue = totalOrders > 0 ? Math.Round(totalSales / totalOrders, 2) : 0
            };

            return new SalesReportResultVM
            {
                Summary = summary,
                Rows = grouped
            };
        }

        // ================== DAY WISE BREAKUP (expandable "View" panel) ==================
        public async Task<List<SalesReportDetailRowVM>> GetSalesReportDetail(string reportType, string periodKey, int shopId)
        {
            DateTime startDate;
            DateTime endDate;

            ParsePeriodKey(reportType, periodKey, out startDate, out endDate);

            var query = _context.tblSaleInvoice
                .Where(d => d.InvoiceDate >= startDate && d.InvoiceDate < endDate);

            if (shopId > 0)
                query = query.Where(d => d.shopid == shopId);

            var details = await query
                .GroupBy(d => d.InvoiceDate.Date)
                .Select(g => new SalesReportDetailRowVM
                {
                    SaleDate = g.Key,
                    InvoiceCount = g.Count(),
                    TotalAmount = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.SaleDate)
                .ToListAsync();

            return details;
        }

        // ================== Helpers ==================

        // reportType + selected date se DB filter ke liye [startDate, endDate) range nikalta hai
        private void GetPeriodRange(string reportType, DateTime date, out DateTime startDate, out DateTime endDate,
            out string periodLabel, out string periodKey)
        {
            switch (reportType)
            {
                case "Week":
                    int year = ISOWeek.GetYear(date);
                    int week = ISOWeek.GetWeekOfYear(date);
                    startDate = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
                    endDate = startDate.AddDays(7);
                    periodLabel = $"{startDate:dd MMM} - {endDate.AddDays(-1):dd MMM yyyy}";
                    periodKey = $"{year}-W{week:D2}";
                    break;

                case "Month":
                    startDate = new DateTime(date.Year, date.Month, 1);
                    endDate = startDate.AddMonths(1);
                    periodLabel = startDate.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
                    periodKey = startDate.ToString("yyyy-MM");
                    break;

                case "Year":
                    startDate = new DateTime(date.Year, 1, 1);
                    endDate = startDate.AddYears(1);
                    periodLabel = startDate.Year.ToString();
                    periodKey = startDate.Year.ToString();
                    break;

                default:
                    throw new ArgumentException("Invalid reportType. Use Week, Month, or Year.");
            }
        }

        // periodKey (jo View button click par frontend se wapas aata hai) se date range reconstruct karta hai
        private void ParsePeriodKey(string reportType, string periodKey, out DateTime startDate, out DateTime endDate)
        {
            switch (reportType)
            {
                case "Week":
                    // format: "2026-W27"
                    var weekParts = periodKey.Split('-');
                    int weekYear = int.Parse(weekParts[0]);
                    int weekNum = int.Parse(weekParts[1].TrimStart('W'));
                    startDate = ISOWeek.ToDateTime(weekYear, weekNum, DayOfWeek.Monday);
                    endDate = startDate.AddDays(7);
                    break;

                case "Month":
                    // format: "2026-07"
                    var monthParts = periodKey.Split('-');
                    startDate = new DateTime(int.Parse(monthParts[0]), int.Parse(monthParts[1]), 1);
                    endDate = startDate.AddMonths(1);
                    break;

                case "Year":
                    // format: "2026"
                    startDate = new DateTime(int.Parse(periodKey), 1, 1);
                    endDate = startDate.AddYears(1);
                    break;

                default:
                    throw new ArgumentException("Invalid reportType. Use Week, Month, or Year.");
            }
        }
    }
}
