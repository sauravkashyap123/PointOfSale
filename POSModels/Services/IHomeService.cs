using Microsoft.AspNetCore.Mvc.Rendering;
using POSModels.Models;
using POSModels.Models.MithaiShop;
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
        public Task<List<ShopModel>> GetAllShopList();
        public Task<AllResponseMessage> SaveShop(ShopModel sh);
        public Task<AllResponseMessage> SaveWarehouse(WarehouseModel wm);
        public Task<AllResponseMessage> SaveCategory(CategoryModel cm);
        public Task<List<CategoryModel>> GetAllCategoryList();
        public Task<AllResponseMessage> SaveUnit(Unit um);
        public List<Unit> GetUnitList();
        public Task<AllResponseMessage> SaveProduct(Product model, string imageurl);
        public Task<bool> CheckCategoryCodeDesc(string categoryCode);
        public Task<List<SelectListItem>?> GetSelectedProductList();
        public Task<List<Product>?> GetAllProductList();
        public Task<AllResponseMessage> SaveProduction(Production pd);
        public Task<List<ProductionList>?> GetAllProductionList();
        public List<StaffModel> GetAllStaffList();
        public Product GetProductDetails(int id);
        public string GetSpecificProductunit(int productId);
        public Task<dynamic> GetAllSendStockTransferList(string warehousecode, DateTime? fromDate, DateTime? toDate);
        //public GeneralModel GetAllDashboardData();
    }
}
