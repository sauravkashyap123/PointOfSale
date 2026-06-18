using Microsoft.AspNetCore.Mvc.Rendering;
using POSModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Services
{
    public interface ISelectItemService
    {
        public List<SelectListItem>? SelectCategory();
        public List<SelectListItem> SelectedWarehouseItem();
        public List<SelectListItem>? SelectUnit();
        public List<SelectListItem>? SelectPurchaseEntryProduct();
        public List<SelectListItem>? SelectShopList();
        public List<SelectListItem>? SelectedProductList();
    }
}
