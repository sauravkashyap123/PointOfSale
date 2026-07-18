using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSModels.Models;
using POSModels.Models.PurchaseEntry;
using POSModels.Services;
using System.Threading.Tasks;

namespace PointOfSale.Controllers
{
    [Authorize(Roles = "admin,Admin")]
    public class PurchaseController : Controller
    {
        private readonly IHomeService _homeservice;
        private readonly ISelectItemService _selectitemservice;
        private readonly IPurchaseService _purchaseservice;
        
        public PurchaseController(IHomeService homeservice,ISelectItemService selectItemService, IPurchaseService purchaseservice)
        {
            _homeservice = homeservice;
            _selectitemservice = selectItemService;
            _purchaseservice = purchaseservice;
        }
        public IActionResult AddPurchaseProduct(int id=0)
        {
            if (id > 0)
            {
                PurchaseProductModel model = new PurchaseProductModel();
                model.SelectedCategoryList = _selectitemservice.SelectCategory();
                model.Id = id;
                model = _purchaseservice.GetPurchaseProductDetail(id);
                model.SelectedCategoryList= _selectitemservice.SelectCategory();
                return View(model);
            }
            else
            {
                PurchaseProductModel model = new PurchaseProductModel();
                model.SelectedCategoryList = _selectitemservice.SelectCategory();
                model.Id = id;
                return View(model);
            }
               
        }
        [HttpPost]
        public async Task<IActionResult> AddPurchaseProduct(PurchaseProductModel ppm)
        {
            AllResponseMessage resp = await _purchaseservice.SavePurchaseProduct(ppm);
            return Ok(new {status=resp.Result,message=resp.Message});
        }
        public IActionResult PurchaseProductList()
        {
            return View();
        }
        public async Task<IActionResult> AllPurchaseProductList()
        {
            var ab = await _purchaseservice.GetAllPurchaseProductList();
            return Ok(new { data = ab });
        }
        public IActionResult PurchaseEntry()
        {
            RawPurchaseModel rawPurchaseModel = new RawPurchaseModel();
            rawPurchaseModel.selectedProductList = _selectitemservice.SelectPurchaseEntryProduct();
            rawPurchaseModel.selectedunitlist = _selectitemservice.SelectUnit();

            return View(rawPurchaseModel);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseEntry(RawPurchaseModel model)
        {
            AllResponseMessage resp = _purchaseservice.SavePurchaseEntry(model);

            if (resp.Result==true)
                TempData["Success"] = resp.Message;
            else
                TempData["Error"] = resp.Message;

            return RedirectToAction("PurchaseEntry");
        }
        public IActionResult AllPurchaseEntryList()
        {
            List<RawPurchaseModel> ab = _purchaseservice.GetAllPurchaseEntryList();
            return View(ab);
        }
        public IActionResult AllPurchaseEntryDetailList(int id)
        {
            var ab = _purchaseservice.GetAllPurchaseEntryDetailList(id);
            return Ok(new { data = ab });
        }
        public IActionResult ViewStock()
        {
            return View();
        }
    }
}
