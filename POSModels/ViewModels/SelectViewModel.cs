using Microsoft.AspNetCore.Mvc.Rendering;
using POSDb.Data;
using POSModels.Models;
using POSModels.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.ViewModels
{
    public class SelectViewModel:ISelectItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly DateTime currentdate = DateTime.Now;

        public SelectViewModel(ApplicationDbContext appContext)
        {
            _context = appContext;
        }
        public List<SelectListItem> SelectedWarehouseItem()
        {
			try
			{
                var ab = _context.tblwarehouse.Select(x => new SelectListItem
                {
                    Text=x.WarehouseName,
                    Value=x.Id.ToString(),
                }).ToList();
                return ab;
			}
			catch (Exception)
			{

				return new List<SelectListItem>();
			}
        }
        public List<SelectListItem>? SelectUnit()
        {
			try
			{
                var ab = _context.tblUnit.Select(x => new SelectListItem
                {
                    Text=x.ShortName+" ( "+x.UnitName+" )",
                    Value=x.Id.ToString(),
                }).ToList();
                return ab;
			}
			catch (Exception)
			{

				return new List<SelectListItem>();
			}
        }
        public List<SelectListItem>? SelectCategory()
        {
			try
			{
                var ab = _context.tblCategory.Select(x => new SelectListItem
                {
                    Text=x.CategoryName+" ( Code= "+x.CategoryCode+" )",
                    Value=x.Id.ToString(),
                }).ToList();
                return ab;
			}
			catch (Exception)
			{

				return new List<SelectListItem>();
			}
        }
        public List<SelectListItem>? SelectPurchaseEntryProduct()
        {
			try
			{
                var ab = _context.tblpurchaseproduct.Select(x => new SelectListItem
                {
                    Text=x.ProductName+" ( Code= "+x.PurchaseProductCode+" )",
                    Value=x.Id.ToString(),
                }).ToList();
                return ab;
			}
			catch (Exception)
			{

				return new List<SelectListItem>();
			}
        }

        public List<SelectListItem>? SelectShopList()
        {
            try
            {
                var ab = (from d in _context.tblShop
                          select new SelectListItem
                          {
                              Text=d.ShopName,
                              Value=d.Id.ToString(),    
                          }).ToList();
                return ab;
            }
            catch (Exception)
            {

                return new List<SelectListItem>();
            }
        }

        public List<SelectListItem>? SelectedProductList()
        {
            try
            {
                var ab = (from d in _context.tblProduct
                          select new SelectListItem
                          {
                              Text = d.ProductName+" ("+d.ProductCode+")",
                              Value=d.Id.ToString(),    
                          }).ToList();
                return ab;
            }
            catch (Exception)
            {

                return new List<SelectListItem>();
            }
        }
    }
}
