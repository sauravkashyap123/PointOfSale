using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using POSDb.Data;
using POSModels.Models;
using POSModels.Models.MithaiShop;
using POSModels.Services;
using System;
using System.Collections.Immutable;
using System.Security;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace PointOfSale.Controllers
{
        [Authorize(Roles = "admin,Admin,staff,Staff")]
        public class BillingController : Controller
        {
            private readonly ApplicationDbContext _db;
            private readonly IBillingService _billing;
            private readonly IHomeService _homeService;
            private readonly ISelectItemService _selectlistitem;
            private readonly ICachedProductService? _cachedProductService;

            public BillingController(ApplicationDbContext db, IBillingService billing, IHomeService homeService, ISelectItemService selectItemService, ICachedProductService? cachedProductService = null)
            {
                _db = db;
                _billing = billing;
                _homeService = homeService;
                _selectlistitem = selectItemService;
                _cachedProductService = cachedProductService;
            }

            public async Task<IActionResult> Index()
            {
            //ViewBag.Products = await _db.Products
            //    .Where(p => p.IsActive && p.Stock > 0)
            //    .OrderBy(p => p.Category).ThenBy(p => p.Name)
            //    .ToListAsync();
            //ViewBag.Customers = await _db.Customers.OrderBy(c => c.Name).ToListAsync();
            //ViewBag.Categories = await _db.Products
            //    .Where(p => p.IsActive)
            //    .Select(p => p.Category)
            //    .Distinct().OrderBy(c => c).ToListAsync();
            //ViewBag.Cart = GetCart();
          
            string userid = User.Identity.Name;
            int shopid = _billing.GetCurrentShopId(userid);

            List<CategoryModel> catls = await _homeService.GetAllCategoryList();
                //List<Product> prols = await _homeService.GetAllProductList();
                List<Product> prols = await _homeService.GetShopwiseAllProductList(shopid);
                List<ShopModel> shls=await _homeService.GetAllShopList();
                ViewBag.Products = prols;
                ViewBag.Shop = shls;
                ViewBag.Categories= catls;
                var user = User.Identity.Name;
                var staffdetails = _billing.GetStaffDetails(User.Identity.Name);
            ViewBag.StaffName = staffdetails.Name;
            ViewBag.StaffShortName = GetShortName(staffdetails.Name);

            var printData = await _db.tblprintdata.FirstOrDefaultAsync(p => p.ShopId == shopid && p.IsActive);
            if (printData != null)
            {
                ViewBag.ShopName = printData.StoreName;
                ViewBag.Mobileno = printData.MobileNo;
                ViewBag.Address = printData.Address;
                ViewBag.Gstnumber = printData.GSTNumber;
            }
            else
            {
                var shop = await _db.tblShop.FirstOrDefaultAsync(s => s.Id == shopid);
                ViewBag.ShopName = shop?.ShopName ?? "Sri Sai Sweets";
                ViewBag.Address = "Main Road, Jamshedpur";
                ViewBag.Mobileno = shop?.ContactNumber ?? "+91 0000000000";
                ViewBag.Gstnumber = "20XXXXXXXXXX";
            }

            return View();
        }

        public IActionResult Terminal()
        {
            return View();
        }

        public string GetShortName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                // Naam ko spaces se todkar words nikalen (e.g., "Arjun", "Kumar")
                var words = name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (words.Length > 1)
                {
                    // Agar ek se zyada words hain (First Name + Last Name), toh dono ka pehla akshar lein
                     return (words[0][0].ToString() + words[words.Length - 1][0].ToString()).ToUpper();
                }
                else if (words.Length == 1 && words[0].Length >= 2)
                {
                    // Agar sirf single name hai (e.g., "Arjun"), toh uske starting ke 2 letters lein
                    return words[0].Substring(0, 2).ToUpper();
                }
                else
                {
                    // Agar naam sirf 1 letter ka hai
                    return words[0].ToUpper();
                }
            }
            else
            {
                return "ST"; // Default agar naam null ya empty ho (Staff)
            }
        }

        public async Task<IActionResult> GetAllProducts()
        {
            List<Product> prols = await _homeService.GetAllProductList();
            return Json(prols);
        }
        public async Task<IActionResult> GetShopWiseAllProducts(int? shopId)
        {
            if (shopId > 0)
            {
                List<Product> prols = await _homeService.GetShopwiseAllProductList(shopId.Value);
                return Ok(prols);
            }
            else
            {
                string userid = User.Identity.Name;
                int shopid = _billing.GetCurrentShopId(userid);

                List<Product> prols = await _homeService.GetShopwiseAllProductList(shopid);
                return Ok(prols);
            }
        }
        // ── CART API ──────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int qty = 1)
        {
            var user = User.Identity.Name;
            var staffdetails = _billing.GetStaffDetails(User.Identity.Name);
            int shopid = staffdetails.Shopid;
            int staffid = staffdetails.Id;
            AllResponseMessage msg =await _billing.SaveCart(productId, qty, staffid, shopid);
            return Ok(new { status = msg.Result,cart=msg.cart,subtotal=msg.cart.Sum(x=>x.Amount), message = msg.Message });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQty(int productId, int qty,string type)
        {
            var user = User.Identity.Name;
            var staffdetails = _billing.GetStaffDetails(User.Identity.Name);
            int shopid = staffdetails.Shopid;
            int staffid = staffdetails.Id;
            //var cart1 = _billing.GetCart(productId, staffid);
            var cart = _billing.SaveUpdatedCart(productId,qty,staffid,type);
            //var item = cart.FirstOrDefault(x => x.ProductId == productId);
            //if (item != null)
            //{
            //    if (qty <= 0) cart.Remove(item);
            //    else item.Quantity = qty;
            //}
            //AllResponseMessage msg = await _billing.SaveUpdateCartQuantity(cart.FirstOrDefault().Id);
            return Json(new { status = cart.Result,message=cart.Message, cart = cart.cart, subtotal = cart.cart.Sum(x => x.Amount) });
        }

        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            var user = User.Identity.Name;
            var staffdetails = _billing.GetStaffDetails(User.Identity.Name);
            int shopid = staffdetails.Shopid;
            int staffid = staffdetails.Id;
            var cart = _billing.RemoveFromCart(productId,staffid);
            
            return Json(new { status = cart.Result,message=cart.Message, cart = cart.cart, subtotal = cart.cart.Sum(x => x.Amount) });
        }



        [HttpPost]
        public IActionResult ClearCart()
        {
            var user = User.Identity.Name;
            var staffdetails = _billing.GetStaffDetails(User.Identity.Name);
            int shopid = staffdetails.Shopid;
            int staffid = staffdetails.Id;
            (List<CartModel> ls, bool status) = _billing.GetAllClearCart(staffid);
            return Json(new {status=status, cart = ls, subtotal = ls.Sum(x => x.Amount), count = ls.Sum(x => x.Quantity) });
        }

        [HttpGet]
        public IActionResult GetCartData()
        {
            var staffdetails = _billing.GetStaffDetails(User.Identity?.Name);

            if (staffdetails == null)
            {
                return Ok(new
                {
                    cart = new List<CartModel>(),
                    subtotal = 0,
                    count = 0
                });
            }

            var cart = _billing.GetAllCart(staffdetails.Id);

            return Ok(new
            {
                cart,
                subtotal = cart.Sum(x => x.Amount),
                count = cart.Sum(x => x.Quantity)
            });
        }

        //// ── BILLING API ──────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> ProcessBill(string? billno, string customername="",string mobileno="", decimal discountPercent=0.00m, string paymentMethod="")
        {
            var user = User.Identity.Name;
            var staffdetails = _billing.GetStaffDetails(User.Identity.Name);
            int shopid = staffdetails.Shopid;
            int staffid = staffdetails.Id;

            var cart = _billing.GetAllCart(staffid);
            if (!cart.Any()) return Json(new { status = false, message = "Cart khaali hai!" });

            try
            {
                var invoice = await _billing.CreateInvoiceAsync(billno,cart, customername, mobileno, discountPercent, paymentMethod,shopid,staffid);
                var clcart = _billing.GetAllClearCart(staffid);

                
                return Ok(new
                {
                    status = true,
                    invoiceId = invoice.Id,
                    invoiceNumber = invoice.InvoiceNumber,
                    total = invoice.TotalAmount,
                    message = $"Bill {invoice.InvoiceNumber} successfully bana!"
                });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });
            }
        }

        public IActionResult GenerateBillNo()
        {
            string billno = _billing.GetGenerateBillNo();  
            return Ok(new {billno= billno });
        }

        [HttpGet]
        public async Task<IActionResult> PrintBill(int invoiceId, string invoiceNumber)
        {
            var invoice = await _billing.GetInvoiceAllDetailsById(invoiceId, invoiceNumber);

            if (invoice == null)
                return NotFound();

            ViewBag.InvoiceNumber = invoiceNumber;

            ViewBag.Mobileno = invoice.PrintMobileno;
            ViewBag.Gstnumber = invoice.GstNumber;
            ViewBag.Address = invoice.PrintAddress;
            ViewBag.ShopName = invoice.ShopName;
            return View(invoice);
        }

        //// ── PRODUCT SEARCH ───────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string q, string? category)
        {
            string userid = User.Identity?.Name;
            int shopid = 0;
            if (!string.IsNullOrEmpty(userid))
            {
                shopid = _billing.GetCurrentShopId(userid);
            }

            var query = from d in _db.tblProduct
                        join c in _db.tblCategory on d.CategoryId equals c.Id
                        join u in _db.tblUnit on d.UnitId equals u.Id
                        join s in _db.tblshopstock.Where(x => x.ShopId == shopid)
                            on d.Id equals s.ProductId into stockGroup
                        from s in stockGroup.DefaultIfEmpty()
                        where d.IsActive == true
                        select new
                        {
                            Id = d.Id,
                            ProductName = d.ProductName,
                            ProductCode = d.ProductCode,
                            SaleRate = d.SaleRate,
                            CategoryName = c.CategoryName,
                            ImageUrl = d.ImageUrl,
                            StockQuantity = s != null ? s.Quantity : 0
                        };

            if (!string.IsNullOrWhiteSpace(category) && category != "Sab kuch")
            {
                query = query.Where(x => x.CategoryName == category);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(x => x.ProductName.Contains(q) || x.ProductCode.Contains(q));
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }

        //// ── INVOICE PRINT ─────────────────────────────────────────
        //public async Task<IActionResult> PrintInvoice(int id)
        //{
        //    var invoice = await _db.Invoices
        //        .Include(i => i.Items)
        //        .FirstOrDefaultAsync(i => i.Id == id);
        //    if (invoice == null) return NotFound();
        //    return View(invoice);
        //}

        //// ── SESSION HELPERS ───────────────────────────────────────
        //private List<CartItem> GetCart()
        //{
        //    var json = HttpContext.Session.GetString(CartSessionKey);
        //    return string.IsNullOrEmpty(json) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(json)!;
        //}

        //private void SaveCart(List<CartItem> cart)
        //{
        //    HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        //}

        public IActionResult ViewStock()
        {
            return View();
        }
            public async Task<IActionResult> AllStockList()
            {
                var ab =await _billing.getAllStockList();
                return Ok(new { data=ab });
            }
            public async Task<IActionResult> ViewStockHistory(int stockid)
            {
                var model =await _billing.Allstockviewhistory(stockid);
                return View(model); 
            }
        //public IActionResult Allstockviewhistory()
        //{
        //    var ab = _billing.GetAllstockviewhistory();

        //    return Ok(new { data = ab });
        //}


        // Print Bill Data
        public IActionResult PrintBilllData(int? id)
        {
            ShopSettingModel model = new ShopSettingModel();

            if (id.HasValue && id > 0)
            {
                model = _billing.GetPrintDataById(id.Value);
            }

            model.selectshoplist = _selectlistitem.SelectShopList();
            

            return View(model);
        }
            [HttpPost]
            public async Task<IActionResult> SaveBillData(ShopSettingModel ssm)
            {
                AllResponseMessage resp =await _billing.PostSaveBillData(ssm);
                return Ok(new { status = resp.Result, message = resp.Message });
            }
            public IActionResult AllPrintDataContent()
            {
                List<ShopSettingModel> ls = _billing.GetAllPrintDataContent();
                return View(ls);
            }

            public IActionResult DeletePrintData(int id)
            {
                _billing.DeletePrintData(id);
                return RedirectToAction("AllPrintDataContent");
            }

           // Shop Sales List

            public IActionResult ShopAllSaleList()
            {
                List<SelectListItem>? selectshop = _selectlistitem.SelectShopList();
                return View(selectshop);
            }
            public async Task<IActionResult> AllItemShopSaleList(int shopid)
            {
                var ab =await _billing.GetAllItemShopSaleList(shopid);
                return Ok(new { data = ab });
            }

            [HttpPost]
            public async Task<IActionResult> CreateBulkOrder([FromBody] BulkOrderModel model)
            {
                if (model == null) return BadRequest("Invalid bulk order data.");

                string username = User.Identity.Name;
                int shopid = _billing.GetCurrentShopId(username);
                var staff = _billing.GetStaffDetails(username);
                int staffid = staff?.Id ?? 0;

                var result = await _billing.CreateBulkOrderAsync(model, shopid, staffid);
                return Json(new { success = result.Result, message = result.Message });
            }
        }
    }     
