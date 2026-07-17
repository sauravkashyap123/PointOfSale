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

        public AllResponseMessage SavePurchaseEntry(RawPurchaseModel model)
		{
            try
            {
                if (model.Items == null || model.Items.Count == 0)
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "At least one item is required."
                    };
                }

                var purchase = new EpurchaseEntry
                {
                    PurchaseNo = model.PurchaseNo,
                    PurchaseDate = model.PurchaseDate,
                    Remarks = model.Remarks,
                    IsDeleted = true,
                    CreatedDate = currentdate
                };

                _context.tblpurchaseentry.Add(purchase);
                _context.SaveChanges();

                foreach (var item in model.Items)
                {
                    if (item.ProductId == 0 || item.Quantity <= 0) continue;

                    decimal baseAmount = item.Quantity * item.Rate;
                    decimal gstAmount = baseAmount * (item.GSTPercent / 100);
                    decimal total = baseAmount + gstAmount;

                    var detail = new EPurchaseDetail
                    {
                        PurchaseId = purchase.Id,
                        ProductId = item.ProductId,
                        UnitId = item.UnitId,
                        Quantity = item.Quantity,
                        Rate = item.Rate,
                        GSTPercent = item.GSTPercent,
                        TotalAmount = total,
                        SubTotal=baseAmount,
                        IsDeleted = false,
                        GSTAmount=gstAmount
                    };

                    _context.tblpurchasedetail.Add(detail);
                }

                _context.SaveChanges();

                return new AllResponseMessage
                {
                    Result = true,
                    Message = "Purchase saved successfully."
                };
            }
            catch (Exception ex)
            {
                return new AllResponseMessage
                {
                    Result = false,
                    Message = "Error: " + ex.Message
                };
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
							eppm.IsActive = true;

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
								eppm.IsActive = true;

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


        public List<RawPurchaseModel> GetAllPurchaseEntryList()
        {
            try
            {
                var list = (from p in _context.tblpurchaseentry
                            
                            join d in _context.tblpurchasedetail
                                on p.Id equals d.PurchaseId into details
                            from d in details.DefaultIfEmpty()
                            group d by new
                            {
                                p.Id,
                                p.PurchaseNo,
                                p.PurchaseDate,
                                p.Remarks,
                                p.CreatedDate
                            } into g
                            orderby g.Key.CreatedDate descending
                            select new RawPurchaseModel
                            {
                                Id = g.Key.Id,
                                PurchaseNo = g.Key.PurchaseNo,
                                PurchaseDate = g.Key.PurchaseDate,
                                Remarks = g.Key.Remarks ?? "-",
                                TotalAmount = g.Sum(x => x != null ? x.TotalAmount : 0),
                                ItemCount = g.Count(x => x != null),
                                CreatedDate = g.Key.CreatedDate
                            }).ToList();

                return list;
            }
            catch (Exception)
            {
                return new List<RawPurchaseModel>();
            }
        }

        public List<RawPurchaseDetailModel> GetAllPurchaseEntryDetailList(int id)
        {
            try
            {
                var ab = (from d in _context.tblpurchasedetail
                          where d.PurchaseId == id
                          join p in _context.tblpurchaseproduct on d.ProductId equals p.Id
                          join u in _context.tblUnit on d.UnitId equals u.Id
                          select new RawPurchaseDetailModel
                          {
                              ProductName=p.ProductName,
                              Rate=d.Rate,
                              Quantity=d.Quantity,
                              UnitName=u.ShortName,
                              GSTPercent=d.GSTPercent,
                              
                          }).ToList();
                return ab;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
