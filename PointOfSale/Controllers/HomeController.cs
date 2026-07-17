using BarcodeStandard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POSDb.EntityModels;
using POSModels.Models;
using POSModels.Models.MithaiShop;
using POSModels.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PointOfSale.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService _homeservice;
        private readonly ISelectItemService _selectitemservice;
        private readonly ILoginViewModels _loginview;
        public readonly ISelectItemService _selectlistitem;
        public readonly IBillingService _billing;

        public HomeController(ILogger<HomeController> logger, IHomeService homeService, ISelectItemService selectitemservice, ILoginViewModels loginview, ISelectItemService selectlistitem, IBillingService billing)
        {
            _logger = logger;
            _homeservice = homeService;
            _selectitemservice = selectitemservice;
            _loginview = loginview;
            _selectlistitem = selectlistitem;
            _billing = billing;
        }
        public IActionResult Index()
        {
            var ab = _homeservice.GetAllDashboardData();
            ViewBag.OperatorName = User.Identity.Name;
            ViewBag.OperatorRole = "Admin";
            return View(ab);
        }
        public IActionResult AddWarehouse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddWarehouse(WarehouseModel wm)
        {
            AllResponseMessage resp = await _loginview.SaveWarehouse(wm);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        public async Task<IActionResult> AllWareHouseList()
        {
            var ab = await _homeservice.GetAllWareHouseList();
            return Ok(new { data = ab });
        }
        [HttpDelete]
        public IActionResult DeleteWarehouse(int id)
        {
            AllResponseMessage resp = _homeservice.DeleteOneWarehouse(id);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AddShop()
        {
            ShopModel sh = new ShopModel();
            sh.selectWarehouse = _selectitemservice.SelectedWarehouseItem();
            return View(sh);
        }
        [HttpPost]
        public async Task<IActionResult> AddShop(ShopModel sh)
        {
            AllResponseMessage resp = await _homeservice.SaveShop(sh);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        public async Task<IActionResult> AllShopList()
        {
            var ab = await _homeservice.GetAllShopList();
            return Ok(new { data = ab });
        }
        [HttpDelete]
        public IActionResult DeleteShop(int id)
        {
            AllResponseMessage resp =  _homeservice.DeleteOneShop(id);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryModel cm)
        {
            AllResponseMessage resp = await _homeservice.SaveCategory(cm);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckCategoryCode(
    string categoryCode)
        {
            bool exists = await _homeservice.CheckCategoryCodeDesc(categoryCode);

            if (exists)
            {
                return Json(
                    "Category Code already exists");
            }

            return Json(true);
        }
        public async Task<IActionResult> AllCategoryList()
        {
            List<CategoryModel> fg = await _homeservice.GetAllCategoryList();
            return Ok(new {data = fg });
        }
        [HttpDelete]
        public IActionResult DeleteCategory(int id)
        {
            AllResponseMessage resp = _homeservice.DeleteOneCategory(id);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        public IActionResult AddUnit()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddUnit(Unit um)
        {
            AllResponseMessage resp = await _homeservice.SaveUnit(um);
            return Ok(new { status = true, message = "Save Successfully" });
        }

        public IActionResult AllUnitList()
        {
            List<Unit> list = _homeservice.GetUnitList();
            return Ok(new { data = list });
        }
        [HttpDelete]
        public IActionResult DeleteUnit(int id)
        {
            AllResponseMessage resp = _homeservice.DeleteOneUnit(id);
            return Ok(new { status = resp.Result, message = resp.Message });
        }

        public IActionResult ChangeUnitStatus(int id)
        {
            AllResponseMessage resp = _homeservice.ChangeUnitStatus(id);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        public IActionResult AddPurchase()
        {
            return View();
        }
        public async Task<IActionResult> AddProduct(int id=0)
        {
            Product model = new Product();

            if (id > 0)
            {
                List<Product> ls = await _homeservice.GetAllProductList();
                model = ls.Where(x => x.Id == id).FirstOrDefault();

                model.SelectUnitList = _selectitemservice.SelectUnit();
                model.SelectedCategoryList = _selectitemservice.SelectCategory();
                return View(model);
            }
            model.SelectUnitList = _selectitemservice.SelectUnit();
            model.SelectedCategoryList = _selectitemservice.SelectCategory();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product model)
        {
            try
            {
                string imageurl = "";
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploadimagesfile");

                    // folder create if not exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // unique file name
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);

                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    // URL path (DB ke liye)
                    imageurl = "/uploadimagesfile/" + fileName;
                }
                AllResponseMessage resp = await _homeservice.SaveProduct(model,imageurl);

                TempData["success"] =
                    "Product Saved Successfully";

                return Ok(new { result = resp.Result, message = resp.Message });
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;

                return Ok(new { result = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            Product model = _homeservice.GetProductDetails(id);
            model.SelectUnitList = _selectitemservice.SelectUnit();
            model.SelectedCategoryList = _selectitemservice.SelectCategory();

            return View("AddProduct", model);
        }


        public async Task<IActionResult> ProductList()
        {
            Product ps=new Product();
            ps.productList = await _homeservice.GetAllProductList();
            return View(ps);
        }
        public IActionResult DeleteProduct(int id)
        {
            AllResponseMessage resp = _homeservice.DeleteOneProduct(id);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        [HttpGet]
        public IActionResult GetProductUnit(int productId)
        {
            var pro = _homeservice.GetSpecificProductunit(productId);
            return Ok(pro);
        }
        public async Task<IActionResult> AddProduction()
        {
            Production pro = new Production();
            pro.SelectedproductList =await _homeservice.GetSelectedProductList();
            return View(pro);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduction(Production pd)
        {
            AllResponseMessage resp = await _homeservice.SaveProduction(pd);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        public async Task<IActionResult> ProductionList()
        {
            List<ProductionList> prolist = await _homeservice.GetAllProductionList();
            return View(prolist);
        }

        
        // Staff Creation

        public IActionResult CreateStaff(int id=0)
        {
            
            StaffModel sf = new StaffModel();
            sf.selectedshoplist = _selectitemservice.SelectShopList();
            return View(sf);
        }
        public IActionResult EditStaff(int id = 0)
        {
            StaffModel sm = _homeservice.GetAllStaffList().FirstOrDefault(c => c.Id == id);
            sm.selectedshoplist = _selectitemservice.SelectShopList();
                
            return View(sm);
        }
        [HttpDelete]
        public ActionResult DeleteStaff(int id)
        {
            AllResponseMessage resp = _homeservice.DeleteOneStaff(id);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        [HttpGet]
        public ActionResult EnabledStaff(int id)
        {
            AllResponseMessage resp = _homeservice.EnabledOneStaff(id);
            return Ok(new { status = resp.Result, message = resp.Message });
        }
        [HttpPost]
        public async Task<IActionResult> CreateStaff(StaffModel sm)
        {
            AllResponseMessage resp = await _loginview.SaveStaff(sm);
            return Ok(new { result = resp.Result, message = resp.Message, Extra = resp.Extra });
        }
        public IActionResult StaffList()
        {
            List<StaffModel> lsm = _homeservice.GetAllStaffList();
            return View(lsm);
        }
        public IActionResult Reports()
        {
            List<SelectListItem>? selectshop = _selectlistitem.SelectShopList();
            return View(selectshop);
        }
        public async Task<IActionResult> AllItemShopSaleList(int shopid)
        {
            var ab = await _billing.GetAllItemShopSaleList(shopid);
            return Ok(new { data = ab });
        }

        public async Task<IActionResult> BulkOrders()
        {
            var orders = await _billing.GetAllBulkOrdersAsync();
            return View(orders);
        }
    }
}