using POSDb.EntityModels;
using POSModels.Models;
using POSModels.Models.MithaiShop;
using POSModels.Models.WarehouseStockTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Services
{
    public interface IBillingService
    {
        public Task<List<StockHistoryVM>> Allstockviewhistory(int stockid);
        public Task<ESalesInvoice> CreateInvoiceAsync(string billno,List<CartModel> cart, string customerName, string mobileNo, decimal discountPercent, string paymentMethod,int shopid,int staffid);
        public List<CartModel> GetAllCart(int staffid);
        public (List<CartModel> ls, bool status) GetAllClearCart(int staffid);
        public Task<List<ShopSaleListVM>> GetAllItemShopSaleList(int shopid);
        public List<ShopSettingModel> GetAllPrintDataContent();
        
        public Task<List<dynamic>> getAllStockList();
        public List<CartModel> GetCart(int productId,int staffid);
        public int GetCurrentShopId(string? userid);
        public string GetGenerateBillNo();
        public Task<InvoicePrintModel?> GetInvoiceAllDetailsById(int invoiceId, string invoiceNumber);
        public ShopSettingModel GetPrintDataById(int value);
        public (string Mobilno,string Gstnumber,string address,string shopname) GetPrintDataShopDetails(string? name);
        public StaffModel GetStaffDetails(string? name);
        public Task<AllResponseMessage> PostSaveBillData(ShopSettingModel ssm);


        public AllResponseMessage RemoveFromCart(int productId, int staffid);
        public Task<AllResponseMessage> SaveCart(int productId, int qty,int staffid,int shopid);
        public Task<AllResponseMessage> SaveStockTransfer(List<StockTransferVM> items, int warehouseid);
        public AllResponseMessage SaveUpdatedCart(int productId, int qty, int staffid,string type);
        public Task<AllResponseMessage> ShopToWarehouseTransferAsync(ShopToWarehouseTransferRequest request);

        public Task<List<StockAdjustmentListDto>> GetAllStockAdjustmentList(string? warehousecode, DateTime? fromDate, DateTime? toDate);
        public Task<AllResponseMessage> CreateBulkOrderAsync(BulkOrderModel model, int shopid, int staffid);
        public Task<List<BulkOrderModel>> GetAllBulkOrdersAsync();
    }
}
