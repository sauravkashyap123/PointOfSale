using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using POSModels.Models.MithaiShop;
using POSModels.Services;
using System.Data;

namespace PointOfSale.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly ISelectItemService _selectItemService;
        private readonly IHomeService _homeservice;
        private readonly IBillingService _billingservice;
        public WarehouseController(ISelectItemService selectItemService, IHomeService service, IBillingService billingService)
        {
            _selectItemService = selectItemService;
            _homeservice = service;
            _billingservice = billingService;
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
        public async Task<JsonResult> StockTransfer([FromBody] List<StockTransferVM> items)
        {
            try
            {
                var warehouseid = 1;
                var ab =await _billingservice.SaveStockTransfer(items,warehouseid);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public IActionResult AllStockTransferList()
        {
            return View();
        }
        public async Task<IActionResult> GetAllStockTransferList(DateTime? fromDate,DateTime? toDate)
        {
            string warehousecode = User.Identity.Name;
            var ab = await _homeservice.GetAllSendStockTransferList(warehousecode,fromDate,toDate);
            return Ok(new { data = ab });
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

