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
                var ab = await _context.tblwarehouse.Where(x=>x.IsActive == true)
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
        public AllResponseMessage DeleteOneWarehouse(int id)
        {
            try
            {
                var ab = (from d in _context.tblwarehouse where d.Id == id select d).FirstOrDefault();
                if (ab != null)
                {
                    ab.IsActive = false;
                    _context.tblwarehouse.Update(ab);
                    _context.SaveChanges();
                    return new AllResponseMessage
                    {
                        Result = true,
                        Message = "Warehouse Deleted Successfully"
                    };

                }
                else
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Warehouse Not Found"
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<ShopModel>> GetAllShopList()
        {
            var data = await (
                from s in _context.tblShop
                join w in _context.tblwarehouse
                on s.WarehouseId equals w.Id
                where s.IActive==true
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

       
        public AllResponseMessage DeleteOneShop(int id)
        {
            try
            {
                var ab = (from d in _context.tblShop where d.Id == id select d).FirstOrDefault();
                if (ab != null)
                {
                    ab.IActive = false;
                    _context.tblShop.Update(ab);
                    _context.SaveChanges();
                    return new AllResponseMessage
                    {
                        Result = true,
                        Message = "Shop Deleted Successfully"
                    };

                }
                else
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Shop Not Found with this ID"
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public AllResponseMessage ChangeUnitStatus(int id)
        {
            try
            {
                var ab = (from d in _context.tblShop where d.Id == id select d).FirstOrDefault();
                if (ab != null)
                {
                    if(ab.IActive==false)  
                        ab.IActive = true;
                    else
                        ab.IActive = false;

                    _context.tblShop.Update(ab);
                    _context.SaveChanges();
                    return new AllResponseMessage
                    {
                        Result = true,
                        Message = "Unit Status Change Successfully"
                    };

                }
                else
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Unit Not Found with this ID"
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }
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
                entity.IsActive = true;

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
                return await _context.tblCategory.Where(x=>x.IsActive==true)
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

        public AllResponseMessage DeleteOneCategory(int id)
        {
            try
            {
                var ab = (from d in _context.tblCategory where d.Id == id select d).FirstOrDefault();
                if (ab != null)
                {
                    ab.IsActive = false;
                    _context.tblCategory.Update(ab);
                    _context.SaveChanges();
                    return new AllResponseMessage { Result = true, Message = "Delete Category Successfully" };
                }
                else
                    return new AllResponseMessage { Result = false, Message = "Data Not Found" };
            }
            catch (Exception)
            {

                return new AllResponseMessage { Result = false, Message = "Some Error OCcured" };
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
                        existingUnit.IsActive = true;

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
                    eu.IsActive = true;

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
                var ab = (from d in _context.tblUnit where d.IsActive == true
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
        public AllResponseMessage DeleteOneUnit(int id)
        {
            try
            {
                var ab = (from d in _context.tblUnit where d.Id == id select d).FirstOrDefault();
                if (ab != null)
                {
                    ab.IsActive = false;
                    _context.tblUnit.Update(ab);
                    _context.SaveChanges();
                    return new AllResponseMessage
                    {
                        Result = true,
                        Message = "Unit Deleted Successfully"
                    };

                }
                else
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Unit Not Found with this ID"
                    };
                }
            }
            catch (Exception)
            {

                throw;
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
                    if (imageUrl != null ||imageUrl!=" ")
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

        public AllResponseMessage DeleteOneProduct(int id)
        {
            try
            {
                var ab = (from d in _context.tblProduct where d.Id == id select d).FirstOrDefault();
                if (ab != null)
                {
                    ab.IsActive= false;
                    _context.tblProduct.Update(ab);
                    _context.SaveChanges();
                    return new AllResponseMessage
                    {
                        Result = true,
                        Message = "Product Deleted Successfully"
                    };
                }
                else
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Product Not Found with this ID"
                    };
                }
            }
            catch (Exception)
            {

                throw;
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
                var prolist =await (from d in _context.tblProduct where d.IsActive == true
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
                                   UnitId=d.UnitId,
                                   
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
        public async Task<List<Product>?> GetShopwiseAllProductList(int shopid)
        {
            try
            {
                var prolist = await (from d in _context.tblProduct
                                     join c in _context.tblCategory on d.CategoryId equals c.Id
                                     join u in _context.tblUnit on d.UnitId equals u.Id
                                     join s in _context.tblshopstock.Where(x => x.ShopId == shopid)
                                         on d.Id equals s.ProductId into stockGroup
                                     from s in stockGroup.DefaultIfEmpty()
                                     where d.IsActive == true
                                     select new Product
                                     {
                                         ProductCode = d.ProductCode,
                                         ProductName = d.ProductName,
                                         CategoryName = c.CategoryName,
                                         CategoryId = c.Id,
                                         UnitName = u.UnitName,
                                         GSTPercent = d.GSTPercent,
                                         PurchaseRate = d.PurchaseRate,
                                         SaleRate = d.SaleRate,
                                         ExpiryDays = d.ExpiryDays,
                                         CreatedDate = d.CreatedDate,
                                         ImageUrl = d.ImageUrl,
                                         Description = d.Description,
                                         Id = d.Id,
                                         UnitId = d.UnitId,
                                         IsWeightMachine = d.IsWeightMachine,
                                         IsPieceWise = d.IsPieceWise,
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
                //var productionexist = await _context.tblProduction
                //    .AnyAsync(d => d.ProductionNo == pd.ProductionNo);

                //if (productionexist)
                //{
                //    resp.Result = false;
                //    resp.Message = "Production no already exist";
                //    return resp;
                //}
                //var productionno = await _context.tblProduction.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                //string prevprodno = productionno.ProductionNo;
                //if (productionno.ProductionNo == null)
                //{
                //    prevprodno = "PN0001";
                //}

                //int number = int.Parse(prevprodno.Substring(2));
                //string next = $"PN{(number + 1):D4}";
                // Format: PN + YearMonthDayHourMinuteSecond (e.g., PN202606271453)
                string nextProductionNo = $"PN{DateTime.Now:yyyyMMddHHmmss}";

                // Output Example: PN20260627145322 (27 June 2026, 2:53:22 PM)

                EProduction ep = new EProduction
                {
                    ProductionNo = nextProductionNo,
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
                              ShopName=s.ShopName,
                              Shopid = s.Id,
                              Id =d.Id,
                              IsActive=d.IsActive
                          }).ToList();
                return ab;
            }
            catch (Exception)
            {

                return new List<StaffModel> { };
            }
        }
        public AllResponseMessage DeleteOneStaff(int id)
        {
            try
            {
                var staff = _context.tblStaff.FirstOrDefault(d => d.Id == id);
                if (staff != null)
                {
                    staff.IsActive = false;
                    _context.tblStaff.Update(staff);

                    var user = _context.Users.FirstOrDefault(x=>x.UserName==staff.StaffCode);
                    if (user != null)
                    {
                        user.LockoutEnabled = false;
                        user.LockoutEnd = DateTimeOffset.MaxValue;
                        _context.Users.Update(user);
                    }

                    _context.SaveChanges();
                    return new AllResponseMessage
                    {
                        Result = true,
                        Message = "Staff deleted successfully"
                    };
                }
                else
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Staff not found with this ID"
                    };
                }
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
        public AllResponseMessage EnabledOneStaff(int id)
        {
            try
            {
                var staff = _context.tblStaff.FirstOrDefault(d => d.Id == id);
                if (staff != null)
                {
                    staff.IsActive = true;
                    _context.tblStaff.Update(staff);

                    var user = _context.Users.FirstOrDefault(x=>x.UserName==staff.StaffCode);
                    if (user != null)
                    {
                        user.LockoutEnabled = true;  // lockout system active
                        user.LockoutEnd = null;       // but abhi locked nahi hai
                        _context.Users.Update(user);
                    }

                    _context.SaveChanges();
                    return new AllResponseMessage
                    {
                        Result = true,
                        Message = "Staff enabled successfully"
                    };
                }
                else
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Staff not found with this ID"
                    };
                }
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
        public Task<AllResponseMessage> SaveWarehouse(WarehouseModel wm)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetAllSendStockTransferList(
      string warehousecode,
      DateTime? fromDate,
      DateTime? toDate)
        {
            var query = from d in _context.tblStockTransfer
                        join wa in _context.tblwarehouse on d.FromShopId equals wa.Id
                        join sh in _context.tblShop on d.ToShopId equals sh.Id
                        join s in _context.tblStockTransferDetails on d.Id equals s.TransferId
                        join p in _context.tblProduct on s.ProductId equals p.Id
                        join u in _context.tblUnit on p.UnitId equals u.Id
                        join c in _context.tblCategory on p.CategoryId equals c.Id
                        select new
                        {
                            warehousename = wa.WarehouseName,
                            warehouseCode = wa.WarehouseCode,
                            shopName = sh.ShopName,
                            productName = p.ProductName,
                            unitName = u.UnitName,
                            categoryName = c.CategoryName,
                            transactionType = d.TransactionType,
                            transferDate = d.TransferDate,
                            quantity = s.Quantity,
                            transferId = s.TransferId
                        };

            if (!string.IsNullOrEmpty(warehousecode))
            {
                var isWarehouseExists = await _context.tblwarehouse.AnyAsync(w => w.WarehouseCode == warehousecode);
                if (isWarehouseExists)
                {
                    query = query.Where(x => x.warehouseCode == warehousecode);
                }
            }

            if (fromDate.HasValue)
                query = query.Where(x => x.transferDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.transferDate.Date <= toDate.Value.Date);

            return await query
                .OrderByDescending(x => x.transferDate)
                .ToListAsync();
        }

        public GeneralModel GetAllDashboardData()
        {
            GeneralModel gm = new GeneralModel();
            try
            {
                var ab = (from d in _context.tblwarehouse select d).ToList();
                var bc = (from d in _context.tblShop select d).ToList();
                var cd = (from d in _context.tblProduct select d).ToList();
                var de = (from d in _context.tblStaff select d).ToList();
                var ef = (from d in _context.tblCategory select d).ToList();

                gm.TotalShop = bc.Count();
                gm.TotalWarehouse = ab.Count();
                gm.TotalProduct = cd.Count();
                gm.TotalStaff = de.Count();
                gm.TotalCategory = ef.Count();

                // Dynamic Dashboard Metrics
                var today = DateTime.Today;

                gm.TotalRevenue = _context.tblSaleInvoice.Any() ? _context.tblSaleInvoice.Sum(x => x.TotalAmount) : 0.00m;
                gm.TotalOrders = _context.tblSaleInvoice.Count();
                
                gm.TodaySales = _context.tblSaleInvoice.Where(x => x.InvoiceDate.Date == today).Any() 
                                ? _context.tblSaleInvoice.Where(x => x.InvoiceDate.Date == today).Sum(x => x.TotalAmount) 
                                : 0.00m;

                var stockValueQuery = (from s in _context.tblstock
                                       join p in _context.tblProduct on s.ProductId equals p.Id
                                       select s.Quantity * p.SaleRate);
                gm.TotalStockValue = stockValueQuery.Any() ? stockValueQuery.Sum() : 0.00m;

                gm.LowStockAlerts = _context.tblstock.Any(x => x.Quantity <= 10) ? _context.tblstock.Count(x => x.Quantity <= 10) : 0;
                gm.ActiveShops = _context.tblShop.Count(x => x.IActive);
                gm.ActiveUsers = _context.tblStaff.Count(x => x.IsActive);

                // Recent Activities Generator
                var recentInvoices = _context.tblSaleInvoice
                    .OrderByDescending(x => x.InvoiceDate)
                    .Take(3)
                    .ToList();
                foreach (var inv in recentInvoices)
                {
                    gm.RecentActivities.Add(new RecentActivityModel
                    {
                        Title = $"Order #{inv.InvoiceNumber} generated",
                        Description = $"Amount: ₹{inv.TotalAmount:N2} · Cust: {inv.CustomerName ?? "Cash"}",
                        TimeAgo = GetTimeAgo(inv.InvoiceDate),
                        Icon = "✅",
                        IconClass = "ic-green"
                    });
                }

                var recentTransfers = _context.tblStockTransfer
                    .OrderByDescending(x => x.TransferDate)
                    .Take(2)
                    .ToList();
                foreach (var st in recentTransfers)
                {
                    gm.RecentActivities.Add(new RecentActivityModel
                    {
                        Title = $"Stock Transfer #{st.Id} sent",
                        Description = $"Date: {st.TransferDate:dd-MM-yyyy}",
                        TimeAgo = GetTimeAgo(st.TransferDate),
                        Icon = "🏭",
                        IconClass = "ic-blue"
                    });
                }

                var recentProductions = _context.tblProduction
                    .OrderByDescending(x => x.ProductionDate)
                    .Take(2)
                    .ToList();
                foreach (var prod in recentProductions)
                {
                    var prodName = _context.tblProduct.Where(x => x.Id == prod.ProductId).Select(z => z.ProductName).FirstOrDefault() ?? "Product";
                    gm.RecentActivities.Add(new RecentActivityModel
                    {
                        Title = $"Production Batch #{prod.ProductionNo} completed",
                        Description = $"{prodName} · Qty: {prod.Quantity}",
                        TimeAgo = GetTimeAgo(prod.ProductionDate),
                        Icon = "⚙️",
                        IconClass = "ic-purple"
                    });
                }

                // If activities are still empty, inject a default welcome activity
                if (gm.RecentActivities.Count == 0)
                {
                    gm.RecentActivities.Add(new RecentActivityModel
                    {
                        Title = "Welcome to Sri Sai Sweets POS System",
                        Description = "Database is set up and all modules are ready for operation.",
                        TimeAgo = "Just now",
                        Icon = "👤",
                        IconClass = "ic-blue"
                    });
                }

                return gm;
            }
            catch (Exception)
            {
                return gm;
            }
        }

        private static string GetTimeAgo(DateTime dt)
        {
            var span = DateTime.Now - dt;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            return $"{(int)span.TotalDays}d ago";
        }
    }
}
