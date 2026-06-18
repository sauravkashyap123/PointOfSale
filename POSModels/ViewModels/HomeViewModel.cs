using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using POSDb.Data;
using POSDb.EntityModels;
using POSDb.EntityModels.MithaiShop;
using POSDb.EntityModels.POSModels.Models;
using POSModels.Models;
using POSModels.Models.MithaiShop;
using POSModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.ViewModels
{
    public class HomeViewModel : IHomeService
    {
        private readonly ApplicationDbContext _context;
        private readonly DateTime currentdate= DateTime.Now;

        public HomeViewModel(ApplicationDbContext appContext)
        {
            _context = appContext;
        }
        public async Task<List<WarehouseModel>> GetAllWareHouseList()
        {
            try
            {
                var ab = await _context.tblwarehouse
                    .Select(x => new WarehouseModel
                    {
                        Address = x.Address,
                        City = x.City,
                        NoOfShop = x.NoOfShop,
                        WarehouseName = x.WarehouseName,
                        ContactNumber = x.ContactNumber,
                        Email = x.Email,
                        Pincode = x.Pincode,
                        WarehouseCode=x.WarehouseCode,
                        Password=x.Password??"",
                        Id=x.Id
                    })
                    .ToListAsync();

                return ab;
            }
            catch (Exception)
            {
                return new List<WarehouseModel>();
            }
        }
        public async Task<List<ShopModel>> GetAllShopList()
        {
            var data = await (
                from s in _context.tblShop
                join w in _context.tblwarehouse
                on s.WarehouseId equals w.Id

                select new ShopModel
                {
                    Id = s.Id,
                    ShopName = s.ShopName,
                    OwnerName = s.OwnerName,
                    ContactNumber = s.ContactNumber,
                    WarehouseId = s.WarehouseId,
                    // Warehouse Name add karo
                    WarehouseName = w.WarehouseName,
                }
            ).ToListAsync();

            return data;
        }

        public async Task<AllResponseMessage> SaveShop(ShopModel model)
        {
            try
            {
                EShop entity;

                // ── UPDATE ────────────────────────────────────────────────────────
                if (model.Id > 0)
                {
                    entity = await _context.tblShop.FindAsync(model.Id);

                    if (entity == null)
                    {
                        return new AllResponseMessage
                        {
                            Result = false,
                            Message = "Shop not found."
                        };
                    }

                    // Warehouse change hui hai to naya limit check karo
                    if (entity.WarehouseId != model.WarehouseId)
                    {
                        var newWarehouse = await _context.tblwarehouse
                            .FirstOrDefaultAsync(x => x.Id == model.WarehouseId);

                        if (newWarehouse == null)
                        {
                            return new AllResponseMessage
                            {
                                Result = false,
                                Message = "Selected warehouse not found."
                            };
                        }

                        // Current shop count in new warehouse (excluding this shop)
                        int countInNew = await _context.tblShop
                            .CountAsync(x => x.WarehouseId == model.WarehouseId
                                          && x.Id != model.Id);

                        if (countInNew >= newWarehouse.NoOfShop)
                        {
                            return new AllResponseMessage
                            {
                                Result = false,
                                Message = $"Shop limit reached for warehouse '{newWarehouse.WarehouseName}'. Max allowed: {newWarehouse.NoOfShop}."
                            };
                        }
                    }

                   
                }

                // ── CREATE ────────────────────────────────────────────────────────
                else
                {
                    var warehouse = await _context.tblwarehouse
                        .FirstOrDefaultAsync(x => x.Id == model.WarehouseId);

                    if (warehouse == null)
                    {
                        return new AllResponseMessage
                        {
                            Result = false,
                            Message = "Selected warehouse not found."
                        };
                    }

                    int currentShopCount = await _context.tblShop
                        .CountAsync(x => x.WarehouseId == model.WarehouseId);

                    if (currentShopCount >= warehouse.NoOfShop)
                    {
                        return new AllResponseMessage
                        {
                            Result = false,
                            Message = $"Shop limit reached for warehouse '{warehouse.WarehouseName}'. Max allowed: {warehouse.NoOfShop}."
                        };
                    }

                    entity = new EShop();
                    

                    await _context.tblShop.AddAsync(entity);
                }

                // ── COMMON FIELDS ─────────────────────────────────────────────────
                entity.ShopName = model.ShopName;
                entity.OwnerName = model.OwnerName;
                entity.ContactNumber = model.ContactNumber;
                entity.WarehouseId = model.WarehouseId;

                await _context.SaveChangesAsync();

                return new AllResponseMessage
                {
                    Result = true,
                    Message = model.Id > 0 ? "Shop updated successfully." : "Shop created successfully."
                };
            }
            catch (Exception ex)
            {
                return new AllResponseMessage
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<AllResponseMessage> SaveCategory(CategoryModel model)
        {
            try
            {
                ECategoryModel entity;

                if (model.Id > 0)
                {
                    entity = await _context.tblCategory.FindAsync(model.Id);

                    if (entity == null)
                    {
                        return new AllResponseMessage
                        {
                            Result = false,
                            Message = "Category not found"
                        };
                    }
                }
               
                else
                {
                    entity = new ECategoryModel();
                }

                entity.CategoryName = model.CategoryName;
                entity.CategoryCode = model.CategoryCode;

                if (model.Id == 0)
                    await _context.tblCategory.AddAsync(entity);

                await _context.SaveChangesAsync();

                return new AllResponseMessage
                {
                    Result = true,
                    Message = model.Id > 0 ? "Updated Successfully" : "Saved Successfully"
                };
            }
            catch (Exception ex)
            {
                return new AllResponseMessage
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<bool> CheckCategoryCodeDesc(string categoryCode)
        {
            try
            {
                //bool exist= await _context.tblCategory
                //.AnyAsync(x => x.CategoryCode == categoryCode);

                bool exist = await _context.tblCategory.AnyAsync(x => x.CategoryCode == categoryCode);
                return exist;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public async Task<List<CategoryModel>> GetAllCategoryList()
        {
            try
            {
                return await _context.tblCategory
                .Select(x => new CategoryModel
                {
                    Id = x.Id,
                    CategoryName = x.CategoryName,
                    CategoryCode=x.CategoryCode
                })
                .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        public async Task<AllResponseMessage> SaveUnit(Unit um)
        {
            AllResponseMessage resp = new AllResponseMessage();

            try
            {
                // UPDATE
                if (um.Id > 0)
                {
                    var existingUnit = await _context.tblUnit
                                                      .FirstOrDefaultAsync(x => x.Id == um.Id);

                    if (existingUnit != null)
                    {
                        existingUnit.UnitName = um.UnitName;
                        existingUnit.ShortName = um.ShortName;

                        _context.tblUnit.Update(existingUnit);

                        await _context.SaveChangesAsync();

                        resp.Result = true;
                        resp.Message = "Unit Updated Successfully";
                    }
                    else
                    {
                        resp.Result = false;
                        resp.Message = "Unit Not Found";
                    }
                }
                // ADD
                else
                {
                    EUnit eu = new EUnit();

                    eu.UnitName = um.UnitName;
                    eu.ShortName = um.ShortName;

                    await _context.tblUnit.AddAsync(eu);

                    await _context.SaveChangesAsync();

                    resp.Result = true;
                    resp.Message = "Unit Saved Successfully";
                }
            }
            catch (Exception ex)
            {
                resp.Result = false;
                resp.Message = ex.Message;
            }

            return resp;
        }

        public List<Unit> GetUnitList()
        {
            try
            {
                var ab = (from d in _context.tblUnit
                          select new Unit
                          {
                              UnitName= d.UnitName,
                              Id=d.Id,
                              ShortName=d.ShortName,
                          }).ToList();
                return ab;
            }
            catch (Exception)
            {

                return new List<Unit>();
            }
        }

        public async Task<AllResponseMessage> SaveProduct(Product model,string imageUrl)
        {
            AllResponseMessage resp = new AllResponseMessage();
            try
            {
                if (model.Id > 0)
                {
                    var ep= (from d in _context.tblProduct where d.ProductCode == model.ProductCode select d).FirstOrDefault();

                    ep.ProductName = model.ProductName;
                    ep.ProductCode = model.ProductCode;
                    ep.CategoryId = model.CategoryId;
                    ep.ProductCode = model.ProductCode;
                    ep.ExpiryDays = model.ExpiryDays;
                    ep.PurchaseRate = Convert.ToDecimal(model.PurchaseRate);
                    ep.SaleRate = model.SaleRate;
                    ep.GSTPercent = model.GSTPercent;
                    ep.UnitId = model.UnitId;
                    ep.Description = model.Description;
                    ep.IsWeightMachine = model.IsWeightMachine;
                    ep.IsPieceWise = model.IsPieceWise;
                    ep.CreatedDate = currentdate;
                    ep.IsDeleted = false;
                    if (imageUrl != null)
                    {
                        ep.ImageUrl = imageUrl;
                    }
                    await _context.SaveChangesAsync();

                    resp.Result = true;
                    resp.Message = "Product Save Successfully";
                    return resp;
                }
                else
                {
                    var proid = (from d in _context.tblProduct where d.ProductCode == model.ProductCode select d).Any();
                    if (proid)
                    {
                        resp.Result = false;
                        resp.Message = "Product Code Already Exist";
                        return resp;
                    }
                    EProduct ep = new EProduct();

                    ep.ProductName = model.ProductName;
                    ep.ProductCode = model.ProductCode;
                    ep.CategoryId = model.CategoryId;
                    ep.ProductCode = model.ProductCode;
                    ep.ExpiryDays = model.ExpiryDays;
                    ep.PurchaseRate = Convert.ToDecimal(model.PurchaseRate);
                    ep.SaleRate = model.SaleRate;
                    ep.GSTPercent = model.GSTPercent;
                    ep.UnitId = model.UnitId;
                    ep.Description = model.Description;
                    ep.IsWeightMachine = model.IsWeightMachine;
                    ep.IsPieceWise = model.IsPieceWise;
                    ep.CreatedDate = currentdate;
                    ep.IsDeleted = false;
                    ep.ImageUrl = imageUrl;

                    _context.tblProduct.Add(ep);

                    await _context.SaveChangesAsync();

                    resp.Result = true;
                    resp.Message = "Product Save Successfully";
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

        public async Task<List<SelectListItem>?> GetSelectedProductList()
        {
            try
            {
                var ab = await (from d in _context.tblProduct
                                select new SelectListItem
                                {
                                    Text = d.ProductName,
                                    Value = d.Id.ToString(),
                                }).ToListAsync();

                return ab;
            }
            catch (Exception)
            {
                return new List<SelectListItem>();
            }
        }

        public async Task<List<Product>?> GetAllProductList()
        {
            try
            {
                var prolist =await (from d in _context.tblProduct
                               join c in _context.tblCategory on d.CategoryId equals c.Id
                               join u in _context.tblUnit on d.UnitId equals u.Id
                                    join s in _context.tblstock
                                    on d.Id equals s.ProductId into stockGroup
                                    from s in stockGroup.DefaultIfEmpty()
                                    select new Product
                               {
                                   ProductCode=d.ProductCode,
                                   ProductName=d.ProductName,
                                   CategoryName=c.CategoryName,
                                   CategoryId=c.Id,
                                   UnitName=u.UnitName,
                                   GSTPercent=d.GSTPercent,
                                   PurchaseRate=d.PurchaseRate,
                                   SaleRate=d.SaleRate,
                                   ExpiryDays=d.ExpiryDays,
                                   CreatedDate=d.CreatedDate,
                                   ImageUrl=d.ImageUrl,
                                   Description=d.Description,
                                   Id=d.Id,
                                   IsWeightMachine=d.IsWeightMachine,
                                   IsPieceWise=d.IsPieceWise,  
                                   StockQuantity = s != null ? s.Quantity : 0,
                               }).ToListAsync();
                return prolist;
            }
            catch (Exception)
            {

                return new List<Product> { };
            }
        }

        public Product GetProductDetails(int id)
        {
            try
            {
                var ab = (from d in _context.tblProduct
                          where d.Id == id
                          select new Product
                          {
                              ProductCode=d.ProductCode,
                              ProductName=d.ProductName,
                              IsPieceWise=d.IsPieceWise,
                              IsWeightMachine=d.IsWeightMachine,
                              CategoryId=d.CategoryId,
                              ExpiryDays=d.ExpiryDays,
                              Description=d.Description,
                              PurchaseRate=d.PurchaseRate,
                              SaleRate=d.SaleRate,
                              UnitId=d.UnitId,
                              GSTPercent=d.GSTPercent,
                              Id=id
                          }).FirstOrDefault();
                return ab;
            }
            catch (Exception)
            {

                return new Product();
            }
        }

        public string GetSpecificProductunit(int productid)
        {
            try
            {
                var ab = (from d in _context.tblProduct join u in _context.tblUnit on d.UnitId equals u.Id where d.Id == productid select u).FirstOrDefault();
                string fullunit = ab.ShortName;
                return fullunit;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task<AllResponseMessage> SaveProduction(Production pd)
        {
            AllResponseMessage resp = new AllResponseMessage();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var productionexist = await _context.tblProduction
                    .AnyAsync(d => d.ProductionNo == pd.ProductionNo);

                if (productionexist)
                {
                    resp.Result = false;
                    resp.Message = "Production no already exist";
                    return resp;
                }

                EProduction ep = new EProduction
                {
                    ProductionNo = pd.ProductionNo,
                    ProductionDate = pd.ProductionDate,
                    ProductId = pd.ProductId,
                    Remarks = pd.Remarks,
                    CostAmount = pd.CostAmount,
                    CreatedDate = currentdate,
                    ExpiryDate = pd.ExpiryDate,
                    Quantity = pd.Quantity
                };

                await _context.tblProduction.AddAsync(ep);

                await SaveStock(pd.ProductId, pd.Quantity);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                resp.Result = true;
                resp.Message = "Production saved successfully";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                resp.Result = false;
                resp.Message = ex.Message;
            }

            return resp;
        }
        private async Task SaveStock(int productId, decimal quantity)
        {
            //using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var stock = await _context.tblstock
                    .FirstOrDefaultAsync(x => x.ProductId == productId);

                if (stock == null)
                {
                    stock = new EStock
                    {
                        ProductId = productId,
                        Quantity = quantity
                    };

                    _context.tblstock.Add(stock);
                    await _context.SaveChangesAsync(); // Id generate karne ke liye
                }
                else
                {
                    stock.Quantity += quantity;
                }

                _context.tblStockHistory.Add(new EStockHistory
                {
                    StockId = stock.Id,
                    Quantity = quantity,
                    Type = "In",
                    EntryDate = currentdate,
                    UpdateDate = currentdate
                });

                await _context.SaveChangesAsync();

                //await transaction.CommitAsync();
            }
            catch
            {
                //await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<ProductionList>?> GetAllProductionList()
        {
            try
            {
                var ab =await (from d in _context.tblProduction
                          join p in _context.tblProduct on d.ProductId equals p.Id
                          join c in _context.tblCategory on p.CategoryId equals c.Id
                          join u in _context.tblUnit on p.UnitId equals u.Id
                          select new ProductionList
                          {
                              ProductionNo = d.ProductionNo,
                              ProductionDate =d.ProductionDate,
                              CostAmount =d.CostAmount,
                              ExpiryDate=d.ExpiryDate,
                              Quantity=d.Quantity,
                              Remarks=d.Remarks,
                              ProductId=d.ProductId,
                              ProductName=p.ProductName,
                              CategoryName=c.CategoryName,
                              Unit=u.UnitName+" ( "+u.ShortName+" )",
                              CreatedDate=d.CreatedDate
                          }).ToListAsync();
                return ab;
            }
            catch (Exception)
            {

                return new List<ProductionList> { };
            }
        }

        // Create Staff

        public List<StaffModel> GetAllStaffList()
        {
            try
            {
                var ab = (from d in _context.tblStaff
                          join s in _context.tblShop on d.Shopid equals s.Id
                          select new StaffModel
                          {
                              Name=d.Name,
                              Mobileno=d.Mobileno,
                              Password=d.Password,
                              Email=d.Emailid,
                              AadharCardNo=d.AadharCardNo,
                              DOJ=d.DOJ,
                              DOB=d.DOB,
                              Address=d.Address,
                              StaffCode=d.StaffCode,
                              ShopName=s.ShopName
                          }).ToList();
                return ab;
            }
            catch (Exception)
            {

                return new List<StaffModel> { };
            }
        }

        public Task<AllResponseMessage> SaveWarehouse(WarehouseModel wm)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GetAllSendStockTransferList(
     string warehousecode,
     DateTime? fromDate,
     DateTime? toDate)
        {
            var query =
                from d in _context.tblStockTransfer
                join wa in _context.tblwarehouse on d.FromShopId equals wa.Id
                join sh in _context.tblShop on d.ToShopId equals sh.Id
                join s in _context.tblStockTransferDetails on d.Id equals s.TransferId
                join p in _context.tblProduct on s.ProductId equals p.Id
                join u in _context.tblUnit on p.UnitId equals u.Id
                join c in _context.tblCategory on p.CategoryId equals c.Id
                where d.TransactionType == "Issue"
                   && (wa.WarehouseCode == warehousecode || warehousecode=="Admin")
                select new
                {
                    Warehousename = wa.WarehouseName,
                    ShopName = sh.ShopName,
                    ProductName = p.ProductName,
                    UnitName = u.UnitName,
                    CategoryName = c.CategoryName,
                    TransactionType = d.TransactionType,
                    TransferDate = d.TransferDate,
                    Quantity = s.Quantity,
                    TransferId = s.TransferId
                };

            if (fromDate.HasValue)
                query = query.Where(x => x.TransferDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.TransferDate.Date <= toDate.Value.Date);

            return await query
                .OrderByDescending(x => x.TransferDate)
                .ToListAsync();
        }



        //public GeneralModel GetAllDashboardData()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


    }
}
