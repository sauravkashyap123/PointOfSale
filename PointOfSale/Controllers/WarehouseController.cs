using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using POSDb.Data;
using POSModels.Models.MithaiShop;
using POSModels.Models.WarehouseStockTransfer;
using POSModels.Services;
using System.Data;

namespace PointOfSale.Controllers
{
    [Authorize(Roles = "admin,Admin,Warehouse Manager,warehouse manager,Warehouse")]
    public class WarehouseController : Controller
    {
        private readonly ISelectItemService _selectItemService;
        private readonly IHomeService _homeservice;
        private readonly IBillingService _billingservice;
        private readonly ApplicationDbContext _context;
        public WarehouseController(ISelectItemService selectItemService, IHomeService service, IBillingService billingService, ApplicationDbContext context)
        {
            _selectItemService = selectItemService;
            _homeservice = service;
            _billingservice = billingService;
            _context = context;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouseDashboardData()
        {
            try
            {
                var warehousecode = User.Identity?.Name;
                var warehouse = await _context.tblwarehouse.FirstOrDefaultAsync(w => w.WarehouseCode == warehousecode);
                if (warehouse == null)
                {
                    return BadRequest("Warehouse not found.");
                }
                var warehouseid = warehouse.Id;

                // 1. Total warehouse stock items (Quantity > 0)
                var activeStockItems = await _context.tblWarehousestock.CountAsync(x => x.WarehouseId == warehouseid && x.Quantity > 0);

                // 2. Total transfers
                var totalTransfers = await _context.tblStockTransfer.CountAsync(x => x.FromShopId == warehouseid);

                // 3. Total adjustments (based on warehouse stock history)
                var totalAdjustments = await _context.tblWarehousestockhistory.CountAsync(x => x.WarehouseId == warehouseid && (x.ReferenceType == "ShopToWarehouseTransfer" || x.Remarks.Contains("Adjustment") || x.Remarks.Contains("Adjust")));

                // 4. Top stock items in this warehouse
                var topStockRaw = await (from s in _context.tblWarehousestock
                                         join p in _context.tblProduct on s.ProductId equals p.Id
                                         where s.WarehouseId == warehouseid && s.Quantity > 0
                                         orderby s.Quantity descending
                                         select new { Product = p.ProductName, Qty = s.Quantity })
                                         .Take(5)
                                         .ToListAsync();

                var stockLabels = topStockRaw.Select(x => x.Product).ToList();
                var stockValues = topStockRaw.Select(x => x.Qty).ToList();

                // 5. Transfer destinations
                var transferDestinations = await (from t in _context.tblStockTransfer
                                                   join sh in _context.tblShop on t.ToShopId equals sh.Id
                                                   where t.FromShopId == warehouseid
                                                   group t by sh.ShopName into g
                                                   select new { Shop = g.Key, Count = g.Count() })
                                                   .ToListAsync();

                var destLabels = transferDestinations.Select(x => x.Shop).ToList();
                var destValues = transferDestinations.Select(x => x.Count).ToList();

                return Json(new
                {
                    activeStockItems = activeStockItems,
                    totalTransfers = totalTransfers,
                    totalAdjustments = totalAdjustments,
                    warehouseName = warehouse.WarehouseName,
                    stock = new { labels = stockLabels, values = stockValues },
                    destinations = new { labels = destLabels, values = destValues }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        public async Task<IActionResult> StockTransfer()
        {
            StockTransfer sm = new StockTransfer();
            sm.selectproduct = _selectItemService.SelectedProductList();
            sm.selectshop=_selectItemService.SelectShopList();
            sm.productlist = await _homeservice.GetAllProductList();
            return View(sm);
        }
        [HttpPost]
        public async Task<IActionResult> StockTransfer([FromBody] List<StockTransferVM> items)
        {
            try
            {
                var warehousecode = User.Identity.Name;
                var warehouse = await _context.tblwarehouse.FirstOrDefaultAsync(w => w.WarehouseCode == warehousecode);
                var warehouseid = warehouse?.Id ?? 1;
                var ab =await _billingservice.SaveStockTransfer(items,warehouseid);

                return Ok(new { status = ab.Result,message=ab.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
        public IActionResult AllStockTransferList()
        {
            return View();
        }
        public async Task<IActionResult> GetAllStockTransferList(DateTime? fromDate, DateTime? toDate)
        {
            string warehousecode = User.Identity.Name;
            var data = await _homeservice.GetAllSendStockTransferList(warehousecode, fromDate, toDate);
            return Ok(new { data = data });
        }
        [HttpGet]
        public IActionResult StockAdjustment()
        {
            var model = new StockTransfer
            {
                selectshop = _selectItemService.SelectShopList(),
                SelectWarehouse = _selectItemService.SelectedWarehouseItem()
            };
            return View(model);
        }
        public IActionResult AllStockAdjustmentList()
        {
            return View();
        }
        public async Task<IActionResult> GetAllStockAdjustmentList(DateTime? fromDate, DateTime? toDate)
        {
            string warehousecode = User.Identity.Name;
            var data = await _billingservice.GetAllStockAdjustmentList(warehousecode, fromDate, toDate);
            return Ok(new { data = data });
        }
        [HttpPost]
        public async Task<JsonResult> SaveStockTransfer([FromBody] ShopToWarehouseTransferRequest request)
        {
            var result = await _billingservice.ShopToWarehouseTransferAsync(request);
            return Json(new { success = result.Result, message = result.Message });
        }
        //[HttpGet]
        //public IActionResult GetShopProducts(int shopId)
        //{
        //    var list = new List<ShopProductDto>();

        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand("sp_GetShopAvailableProducts", con)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    cmd.Parameters.AddWithValue("@ShopId", shopId);
        //    con.Open();

        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //    {
        //        list.Add(new ShopProductDto
        //        {
        //            ProductId = Convert.ToInt32(dr["ProductId"]),
        //            ProductName = dr["ProductName"].ToString()!,
        //            CategoryName = dr["CategoryName"].ToString()!,
        //            UnitName = dr["UnitName"].ToString()!,
        //            AvailableQty = Convert.ToDecimal(dr["AvailableQty"]),
        //            ImagePath = dr["ImagePath"] == DBNull.Value
        //                               ? null
        //                               : dr["ImagePath"].ToString()
        //        });
        //    }

        //    return Json(list);
        //}

        //// ── GET: /Warehouse/GetWarehouseProducts?warehouseId=1 ──────────
        //// Returns products available in a warehouse (for Warehouse→Shop transfer)
        //[HttpGet]
        //public IActionResult GetWarehouseProducts(int warehouseId)
        //{
        //    var list = new List<ShopProductDto>();

        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand("sp_GetWarehouseAvailableProducts", con)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
        //    con.Open();

        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //    {
        //        list.Add(new ShopProductDto
        //        {
        //            ProductId = Convert.ToInt32(dr["ProductId"]),
        //            ProductName = dr["ProductName"].ToString()!,
        //            CategoryName = dr["CategoryName"].ToString()!,
        //            UnitName = dr["UnitName"].ToString()!,
        //            AvailableQty = Convert.ToDecimal(dr["AvailableQty"]),
        //            ImagePath = dr["ImagePath"] == DBNull.Value
        //                               ? null
        //                               : dr["ImagePath"].ToString()
        //        });
        //    }

        //    return Json(list);
        //}

        //// ── POST: /Warehouse/SaveStockTransfer ───────────────────────────
        //[HttpPost]
        //public IActionResult SaveStockTransfer([FromBody] SaveTransferRequest req)
        //{
        //    if (req.Items == null || req.Items.Count == 0)
        //        return Json(new { success = false, message = "Koi item select nahi kiya." });

        //    try
        //    {
        //        using var con = new SqlConnection(_conn);
        //        con.Open();

        //        // Build a DataTable to pass as TVP (Table-Valued Parameter)
        //        var dt = new DataTable();
        //        dt.Columns.Add("ProductId", typeof(int));
        //        dt.Columns.Add("Quantity", typeof(decimal));
        //        foreach (var item in req.Items)
        //            dt.Rows.Add(item.ProductId, item.Quantity);

        //        using var cmd = new SqlCommand("sp_SaveStockTransfer", con)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        cmd.Parameters.AddWithValue("@TransferType", req.TransferType);
        //        cmd.Parameters.AddWithValue("@FromShopId", (object?)req.FromShopId ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@FromWarehouseId", (object?)req.FromWarehouseId ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@ToShopId", (object?)req.ToShopId ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@ToWarehouseId", (object?)req.ToWarehouseId ?? DBNull.Value);

        //        // TVP parameter — type name must match SQL USER-DEFINED TABLE TYPE
        //        var tvp = cmd.Parameters.AddWithValue("@Items", dt);
        //        tvp.SqlDbType = SqlDbType.Structured;
        //        tvp.TypeName = "dbo.TransferItemType";

        //        cmd.ExecuteNonQuery();

        //        return Json(new { success = true, message = "Transfer successfully save ho gaya!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Error: " + ex.Message });
        //    }
        //}
    }


}

