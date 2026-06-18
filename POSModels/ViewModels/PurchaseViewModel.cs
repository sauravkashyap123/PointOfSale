using Microsoft.EntityFrameworkCore;
using POSDb.Data;
using POSDb.EntityModels.PurchaseEntry;
using POSModels.Models;
using POSModels.Models.PurchaseEntry;
using POSModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.ViewModels
{
    public class PurchaseViewModel:IPurchaseService
    {
		private readonly DateTime currentdate=DateTime.Now;
		private readonly ApplicationDbContext _context;
		public PurchaseViewModel(ApplicationDbContext context)
		{
			_context=context;
		}
        public PurchaseProductModel GetPurchaseProductDetail(int id)
		{
			try
			{
				var ab = (from d in _context.tblpurchaseproduct
						  where d.Id == id
						  select new PurchaseProductModel
						  {
							  PurchaseProductCode=d.PurchaseProductCode,
							  ProductName=d.ProductName,
							  CategoryId=d.CategoryId,
							  Warehouseid=d.Warehouseid,
							  Id=id,
							  
						  }).FirstOrDefault();
				
				return ab;
			}
			catch (Exception)
			{

				throw;
			}
		}
        public async Task<AllResponseMessage> SavePurchaseProduct(PurchaseProductModel ppm)
        {
			AllResponseMessage resp=new AllResponseMessage();
			try
			{
				if (ppm.Id > 0)
				{
                    

                    var eppm = await (from d in _context.tblpurchaseproduct where d.Id==ppm.Id select d).FirstOrDefaultAsync();
					if (eppm != null)
					{
						if (eppm.PurchaseProductCode == ppm.PurchaseProductCode)
						{
                            eppm.CategoryId = ppm.CategoryId;
                            eppm.ProductName = ppm.ProductName;
                            eppm.PurchaseProductCode = ppm.PurchaseProductCode;
                            eppm.Warehouseid = ppm.Warehouseid;
                            eppm.CreatedDate = currentdate;


                            await _context.SaveChangesAsync();
                        }
						else
						{
                            var procodeexist = await (from d in _context.tblpurchaseproduct where d.PurchaseProductCode == ppm.PurchaseProductCode && d.Id != ppm.Id select d).ToListAsync();
                            if (procodeexist.Count > 0)
                            {
                                resp.Result = false;
                                resp.Message = "Product Code already exist to another product";
                                return resp;
                            }
							else
							{
                                eppm.CategoryId = ppm.CategoryId;
                                eppm.ProductName = ppm.ProductName;
                                eppm.PurchaseProductCode = ppm.PurchaseProductCode;
                                eppm.Warehouseid = ppm.Warehouseid;
                                eppm.CreatedDate = currentdate;


                                await _context.SaveChangesAsync();
                            }
                        }
                        
                    }
                    resp.Result = true;
                    resp.Message = "Purchase Product Added Successfully";
                    return resp;
                }
				else
				{
					var procodeexist =await (from d in _context.tblpurchaseproduct where d.PurchaseProductCode == ppm.PurchaseProductCode select d).ToListAsync();
					if(procodeexist.Count > 0)
					{
                        resp.Result = false;
                        resp.Message = "Product Code already exist";
                        return resp;
                    }
					EPurchaseProductModel eppm=new EPurchaseProductModel();
					eppm.CategoryId= ppm.CategoryId;	
					eppm.ProductName= ppm.ProductName;	
					eppm.PurchaseProductCode= ppm.PurchaseProductCode;
					eppm.Warehouseid= ppm.Warehouseid;
					eppm.CreatedDate = currentdate;

					await _context.tblpurchaseproduct.AddAsync(eppm);
					await _context.SaveChangesAsync();

					resp.Result = true;
					resp.Message = "Save Purchase product Successfully";
					return resp;
				}
			}
			catch (Exception ex)
			{
                resp.Result = false;
                resp.Message = ex.Message;
                return resp;
            }
        }

        public async Task<dynamic> GetAllPurchaseProductList()
        {
			try
			{
				var ab =await (from d in _context.tblpurchaseproduct
						  join c in _context.tblCategory on d.CategoryId equals c.Id
                               join w in _context.tblwarehouse on d.Warehouseid equals w.Id into wh
                               from w in wh.DefaultIfEmpty()
                               select new
						  {
							  d.ProductName,
							  d.Id,
							  d.Warehouseid,

							  WarehouseName = w != null ? w.WarehouseName : "admin",
                                   d.CreatedDate,
							  d.PurchaseProductCode,
							  c.CategoryName,

						  }).ToListAsync();
				return ab;
			}
			catch (Exception)
			{

				throw;
			}
        }
    }
}
