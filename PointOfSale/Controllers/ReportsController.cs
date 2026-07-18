using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POSDb.Data;
using POSModels.Services;

namespace PointOfSale.Controllers
{
    [Authorize(Roles = "admin,Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ApplicationDbContext _context;   // TODO: shop dropdown populate karne ke liye (ya apni existing shop service use kar lo)

        public ReportsController(IReportService reportService, ApplicationDbContext context)
        {
            _reportService = reportService;
            _context = context;
        }

        // GET: /Reports
        public async Task<IActionResult> Index()
        {
            var shops = await _context.tblShop
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.ShopName
                })
                .ToListAsync();

            return View(shops);
        }

        // GET: /Reports/GetSalesReport?shopId=0&reportType=Week&date=2026-06-29
        [HttpGet]
        public async Task<IActionResult> GetSalesReport(int shopId, string reportType, DateTime date)
        {
            var result = await _reportService.GetSalesReport(shopId, reportType, date);
            return Ok(result);
        }

        // GET: /Reports/GetSalesReportDetail?reportType=Week&periodKey=2026-W27&shopId=1
        [HttpGet]
        public async Task<IActionResult> GetSalesReportDetail(string reportType, string periodKey, int shopId)
        {
            var details = await _reportService.GetSalesReportDetail(reportType, periodKey, shopId);
            return Ok(new { details });
        }

        // GET: /Reports/ExportSalesReport?shopId=0&reportType=Month&date=2026-07-01
        // TODO: CSV/Excel export chahiye to bata dena, main ClosedXML/CsvHelper se likh dunga
        [HttpGet]
        public async Task<IActionResult> ExportSalesReport(int shopId, string reportType, DateTime date)
        {
            var result = await _reportService.GetSalesReport(shopId, reportType, date);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Period,Shop,Total Orders,Total Sales,Avg Order Value");
            foreach (var row in result.Rows)
            {
                sb.AppendLine($"{row.PeriodLabel},{row.ShopName},{row.TotalOrders},{row.TotalSales},{row.AvgOrderValue}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"SalesReport_{reportType}_{date:yyyyMMdd}.csv");
        }
    }
}
