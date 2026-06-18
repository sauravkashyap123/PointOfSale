using POSModels.Models;
using POSModels.Models.PurchaseEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Services
{
    public interface IPurchaseService
    {
        public Task<dynamic> GetAllPurchaseProductList();
        public PurchaseProductModel GetPurchaseProductDetail(int id);
        public Task<AllResponseMessage> SavePurchaseProduct(PurchaseProductModel ppm);
    }
}
