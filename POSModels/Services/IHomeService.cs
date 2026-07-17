using Microsoft.AspNetCore.Mvc.Rendering;
using POSModels.Models;
using POSModels.Models.MithaiShop;
using POSModels.Models.PurchaseEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Services
{
    public interface IHomeService
    {
        public Task<List<WarehouseModel>> GetAllWareHouseList();
        public AllResponseMessage DeleteOneWarehouse(int id);
        public Task<List<ShopModel>> GetAllShopList();
        public Task<AllResponseMessage> SaveShop(ShopModel sh);
        public Task<AllResponseMessage> SaveWarehouse(WarehouseModel wm);
        public Task<AllResponseMessage> SaveCategory(CategoryModel cm);
        public Task<List<CategoryModel>> GetAllCategoryList();
        public AllResponseMessage DeleteOneCategory(int id);
        public Task<AllResponseMessage> SaveUnit(Unit um);
        public List<Unit> GetUnitList();
        public AllResponseMessage DeleteOneUnit(int id);
        public Task<AllResponseMessage> SaveProduct(Product model, string imageurl);
        public AllResponseMessage DeleteOneProduct(int id);
        public Task<bool> CheckCategoryCodeDesc(string categoryCode);
        public Task<List<SelectListItem>?> GetSelectedProductList();
        public Task<List<Product>?> GetAllProductList();
        public Task<AllResponseMessage> SaveProduction(Production pd);
        public Task<List<ProductionList>?> GetAllProductionList();
        public List<StaffModel> GetAllStaffList();
        public AllResponseMessage DeleteOneStaff(int id);
        public AllResponseMessage EnabledOneStaff(int id);
        public Product GetProductDetails(int id);
        public string GetSpecificProductunit(int productId);
        public Task<IEnumerable<object>> GetAllSendStockTransferList(string warehousecode, DateTime? fromDate, DateTime? toDate);
        public GeneralModel GetAllDashboardData();
        public AllResponseMessage DeleteOneShop(int id);
        public AllResponseMessage ChangeUnitStatus(int id);
        public Task<List<Product>?> GetShopwiseAllProductList(int shopid);


    }
}
