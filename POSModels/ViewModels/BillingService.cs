using Microsoft.EntityFrameworkCore;
using POSDb.Data;
using POSDb.EntityModels;
using POSDb.EntityModels.MithaiShop;
using POSModels.Models;
using POSModels.Models.MithaiShop;
using POSModels.Models.WarehouseStockTransfer;
using POSModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.ViewModels
{
    public class BillingService : IBillingService
    {
		public readonly ApplicationDbContext _context;
		public readonly DateTime createddate=DateTime.Now;
        
		public BillingService(ApplicationDbContext context)
		{
			_context = context;
		}

        public async Task<List<StockHistoryVM>> Allstockviewhistory(int stockid)
        {
            return await (
                from d in _context.tblstock
                where d.Id == stockid
                join s in _context.tblStockHistory on d.Id equals s.StockId
                join p in _context.tblProduct on d.ProductId equals p.Id
                join c in _context.tblCategory on p.CategoryId equals c.Id
                select new StockHistoryVM
                {
                    Quantity = s.Quantity,
                    Type = s.Type,
                    ProductName = p.ProductName,
                    ProductCode = p.ProductCode,
                    EntryDate = s.EntryDate,
                    CategoryName = c.CategoryName
                }
            ).ToListAsync();
        }
        public async Task<ESalesInvoice> CreateInvoiceAsync(string billno,List<CartModel> cart, string customerName,string mobileNo,decimal discountPercent,string paymentMethod,int shopid,int staffid)
        {
            if (cart == null || !cart.Any())
                throw new Exception("Cart is empty.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                decimal grossAmount = cart.Sum(x => x.Amount);
                decimal discountAmount = grossAmount * discountPercent / 100;
                decimal netAmount = grossAmount - discountAmount;

                string invoiceNo = billno;

                var invoice = new ESalesInvoice
                {
                    InvoiceNumber = invoiceNo,
                    CustomerName = customerName,
                    MobileNo = mobileNo??" ",
                    GrossAmount = grossAmount,
                    DiscountPercent = discountPercent,
                    DiscountAmount = discountAmount,
                    TotalAmount = netAmount,
                    PaymentMethod = paymentMethod,
                    InvoiceDate = DateTime.Now,
                    shopid = shopid,
                    staffid = staffid
                };

                 _context.tblSaleInvoice.Add(invoice);
                await _context.SaveChangesAsync();

                foreach (var item in cart)
                {
                    var detail = new ESalesInvoiceDetail
                    {
                        InvoiceId = invoice.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Rate = item.Rate,
                        Amount = item.Amount
                    };

                    _context.tblSaleDetailsInvoice.Add(detail);

                    // Stock Deduct
                    var stock = await _context.tblstock
                        .FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                    if (stock != null)
                    {
                        stock.Quantity -= item.Quantity;

                        if (stock.Quantity < 0)
                            throw new Exception($"Insufficient stock for Product Id {item.ProductId}");
                    }
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return invoice;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<dynamic>> getAllStockList()
        {
            try
            {
                var data = await(
                    from d in _context.tblstock
                    join p in _context.tblProduct on d.ProductId equals p.Id
                    join u in _context.tblUnit on p.UnitId equals u.Id
                    join c in _context.tblCategory on p.CategoryId equals c.Id
                    select new
                    {
                        d.Id,
                        d.ProductId,
                        p.ProductName,
                        p.ProductCode,
                        p.CategoryId,
                        c.CategoryName,
                        d.Quantity,
                        u.UnitName,
                        u.ShortName,
                    }
                ).ToListAsync();

                return data.Cast<dynamic>().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CartModel> GetAllCart(int staffid)
        {
            try
            {
                var existingcart1 = (from d in _context.tblCart where d.StaffId==staffid
                                    
                                     join product in _context.tblProduct on d.ProductId equals product.Id
                                     select new CartModel
                                     {
                                        
                                         ProductId = product.Id,
                                         ProductName = product.ProductName,
                                         Quantity = d.Quantity,
                                         Rate = product.SaleRate,
                                         CreatedDate = createddate,
                                         Amount = d.Amount,

                                     }).ToList();

                List<CartModel> ls = existingcart1;
                return ls;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public (List<CartModel> ls, bool status) GetAllClearCart(int staffid)
        {
            bool result = false;
            try
            {
                var cartItems = _context.tblCart
                    .Where(x => x.StaffId == staffid)
                    .ToList();

                if (cartItems.Any())
                {
                    _context.tblCart.RemoveRange(cartItems);
                    _context.SaveChanges();
                    result = true;  
                }

                return (new List<CartModel>(),result);
            }
            catch (Exception)
            {

                return (new List<CartModel>(),result);
            }
        }

        public async Task<AllResponseMessage> SaveCart(int productId, int qty, int staffid,int shopid)
        {
            try
            {
                var stock = await _context.tblstock
                    .FirstOrDefaultAsync(x => x.ProductId == productId);

                if (stock == null)
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Stock record nahi mila"
                    };
                }

                var product = await (from p in _context.tblProduct join u in _context.tblUnit on p.UnitId equals u.Id where p.Id == productId
                                    select new
                                    {
                                        ProductId = p.Id,
                                        ProductName = p.ProductName,
                                        p.SaleRate,
                                        p.PurchaseRate,
                                        p.UnitId,
                                        UnitName = u.UnitName,
                                        p.ProductCode,

                                    }
                                ).FirstOrDefaultAsync();

                if (product == null)
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Product nahi mila"
                    };
                }

                if (stock.Quantity < qty)
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Stock available nahi hai"
                    };
                }

                var existingCart = await _context.tblCart
                    .FirstOrDefaultAsync(x => x.StaffId == staffid &&
                                              x.ProductId == productId);

                if (existingCart != null)
                {
                    existingCart.Quantity += qty;
                    existingCart.Amount = existingCart.Quantity * existingCart.Rate;
                    _context.tblCart.Update(existingCart);
                }
                else
                {
                    var cartItem = new ECart
                    {
                        StaffId = staffid,
                        Shopid = shopid,
                        ProductId = productId,
                        ProductName = product.ProductName,
                        Quantity = qty,
                        Rate = product.SaleRate,
                        CreatedDate = createddate,
                        Amount= qty*product.SaleRate,
                        ProductCode=product.ProductCode,
                        UnitName=product.UnitName
                    };

                    await _context.tblCart.AddAsync(cartItem);
                }


                await _context.SaveChangesAsync();

                var existingcart1 = (from d in _context.tblCart where d.StaffId == staffid join s in _context.tblShop on d.Shopid equals s.Id select new CartModel
                {
                    StaffId = staffid,
                    ShopName = s.ShopName,
                    ProductId = productId,
                    ProductName = product.ProductName,
                    Quantity = qty,
                    Rate = product.SaleRate,
                    CreatedDate = createddate,
                    Amount = qty * product.SaleRate,
                    ProductCode = product.ProductCode,
                    UnitName = product.UnitName

                }).ToList();

                List<CartModel> ls = existingcart1;
                
                return new AllResponseMessage
                {
                    Result = true,
                    Message = "Cart me successfully add ho gaya",
                    cart=ls,
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

        public async Task<AllResponseMessage> SaveStockTransfer(List<StockTransferVM> items, int warehouseid)
        {
            AllResponseMessage resp = new AllResponseMessage();

            try
            {
                List<string> errorLogs = new();
                int successCount = 0;

                foreach (var item in items)
                {
                    var productname =await _context.tblProduct.Where(x=>x.Id==item.productid).FirstOrDefaultAsync();
                    var stock = await _context.tblstock
                        .FirstOrDefaultAsync(x => x.ProductId == item.productid);

                    if (stock == null)
                    {
                        errorLogs.Add(
                            $"Product Name {productname.ProductName} : Stock record not found.");
                        continue;
                    }

                    if (stock.Quantity < item.quantity)
                    {
                        errorLogs.Add(
                            $"Product Id {productname.ProductName} : Requested {item.quantity}, Available {stock.Quantity}");
                        continue;
                    }

                    // Warehouse Stock Deduct
                    stock.Quantity -= item.quantity;

                    await _context.tblStockHistory.AddAsync(new EStockHistory
                    {
                        StockId = stock.Id,
                        Quantity = item.quantity,
                        Type = "Out",
                        EntryDate = createddate
                    });

                    // Transfer Header
                    var transfer = new EStockTransfer
                    {
                        FromShopId = warehouseid,
                        ToShopId = item.shopid,
                        TransferDate = createddate,
                        TransactionType = "Issue",
                        Remarks = "Stock Transferred"
                    };

                    await _context.tblStockTransfer.AddAsync(transfer);
                    await _context.SaveChangesAsync();

                    // Transfer Detail
                    await _context.tblStockTransferDetails.AddAsync(new EStockTransferDetail
                    {
                        TransferId = transfer.Id,
                        ProductId = item.productid,
                        Quantity = item.quantity,
                        CreatedDate = createddate
                    });

                    // Shop Stock Update
                    var shopStock = await _context.tblshopstock
                        .FirstOrDefaultAsync(x =>
                            x.ShopId == item.shopid &&
                            x.ProductId == item.productid);

                    if (shopStock == null)
                    {
                        shopStock = new EShopStock
                        {
                            ShopId = item.shopid,
                            ProductId = item.productid,
                            Quantity = item.quantity
                        };

                        await _context.tblshopstock.AddAsync(shopStock);
                        await _context.SaveChangesAsync();

                        await _context.tblshopstockHistory.AddAsync(new EShopStockHistory
                        {
                            Shopstockid = shopStock.Id,
                            ShopId = item.shopid,
                            ProductId = item.productid,
                            Quantity = item.quantity,
                            Type = "In",
                            EntryDate = createddate
                        });
                    }
                    else
                    {
                        shopStock.Quantity += item.quantity;

                        await _context.tblshopstockHistory.AddAsync(new EShopStockHistory
                        {
                            Shopstockid = shopStock.Id,
                            ShopId = item.shopid,
                            ProductId = item.productid,
                            Quantity = item.quantity,
                            Type = "In",
                            EntryDate = createddate
                        });
                    }

                    successCount++;
                }

                await _context.SaveChangesAsync();

                resp.Result = successCount > 0;

                if (errorLogs.Any())
                {
                    resp.Message =
                        $"Transferred Products : {successCount}\n\nFailed Products:\n" +
                        string.Join("\n", errorLogs);
                }
                else
                {
                    resp.Result = true;
                    resp.Message =
                        $"All {successCount} products transferred successfully.";
                }

                return resp;
            }
            catch (Exception ex)
            {
                resp.Result = false;
                resp.Message = ex.Message;
                return resp;
            }
        }

        public AllResponseMessage SaveUpdatedCart(int productId, int qty, int staffid, string type)
        {
            AllResponseMessage resp = new AllResponseMessage();

            try
            {
                var existingCart = _context.tblCart
                    .FirstOrDefault(x => x.ProductId == productId && x.StaffId == staffid);

                if (existingCart == null)
                {
                    resp.Result = false;
                    resp.Message = "Cart item not found.";
                    return resp;
                }

                var stock = _context.tblstock
                    .FirstOrDefault(x => x.ProductId == productId);

                if (stock == null)
                {
                    resp.Result = false;
                    resp.Message = "Stock not found.";
                    return resp;
                }

                if (type == "increase")
                {
                    //if (stock.Quantity >= (existingCart.Quantity + qty))
                    if (stock.Quantity >= ( qty))
                    {
                        existingCart.Quantity = qty;
                        existingCart.Amount = existingCart.Quantity * existingCart.Rate;
                    }
                    else
                    {
                        resp.Result = false;
                        resp.Message = "Insufficient stock.";
                        return resp;
                    }
                }
                else if (type == "decrease")
                {
                    existingCart.Quantity =existingCart.Quantity-1;

                    if (existingCart.Quantity <= 0)
                    {
                        _context.tblCart.Remove(existingCart);
                    }
                }

                _context.SaveChanges();

                // Updated Cart List
                var cartList = (from c in _context.tblCart
                                join p in _context.tblProduct
                                    on c.ProductId equals p.Id
                                where c.StaffId == staffid
                                select new CartModel
                                {
                                    Id=c.Id,
                                    ProductId=c.ProductId,
                                    ProductName = p.ProductName,
                                    Quantity=c.Quantity,
                                    Rate=p.SaleRate,
                                    Amount = c.Amount,
                                }).ToList();

                resp.Result = true;
                resp.Message = "Cart updated successfully.";
                resp.cart = cartList;

                return resp;
            }
            catch (Exception ex)
            {
                resp.Result = false;
                resp.Message = ex.Message;
                return resp;
            }
        }

        public AllResponseMessage RemoveFromCart(int productId, int staffid)
        {
            AllResponseMessage resp = new AllResponseMessage();

            try
            {
                var cartItem = _context.tblCart
                    .FirstOrDefault(x => x.ProductId == productId && x.StaffId == staffid);

                if (cartItem == null)
                {
                    resp.Result = false;
                    resp.Message = "Cart item not found.";
                    resp.cart = new List<CartModel>();
                    return resp;
                }

                _context.tblCart.Remove(cartItem);
                _context.SaveChanges();

                var cartList = (from c in _context.tblCart
                                join p in _context.tblProduct
                                    on c.ProductId equals p.Id
                                where c.StaffId == staffid
                                select new CartModel
                                {
                                    Id = c.Id,
                                    ProductId = c.ProductId,
                                    ProductName = p.ProductName,
                                    Quantity = c.Quantity,
                                    Rate = p.SaleRate,
                                    Amount = c.Quantity * p.SaleRate
                                }).ToList();

                resp.Result = true;
                resp.Message = "Item removed from cart successfully.";
                resp.cart = cartList;

                return resp;
            }
            catch (Exception ex)
            {
                resp.Result = false;
                resp.Message = ex.Message;
                resp.cart = new List<CartModel>();
                return resp;
            }
        }

        public List<CartModel> GetCart(int productId,int staffid)
        {
            try
            {
                var existingcart1 = (from d in _context.tblCart
                                          where productId==d.ProductId
                                          join product in _context.tblProduct on d.ProductId equals product.Id
                                     where d.StaffId==staffid
                                          select new CartModel
                                          {
                                              StaffId = staffid,
                                              ProductId = productId,
                                              ProductName = product.ProductName,
                                              Quantity = d.Quantity,
                                              Rate = product.SaleRate,
                                              CreatedDate = createddate,
                                              Amount = d.Amount,
                                              Id=d.Id,

                                          }).ToList();

                List<CartModel> ls = existingcart1;
                return ls;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int GetCurrentShopId(string? userid)
        {
            try
            {
                int shopid = (from d in _context.tblStaff where d.StaffCode == userid select d.Shopid).FirstOrDefault();
                return shopid;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<InvoicePrintModel?> GetInvoiceAllDetailsById(int invoiceId,string invoiceNumber)
        {
            try
            {
                var dbInvoice = await _context.tblSaleInvoice
                    .FirstOrDefaultAsync(x => x.Id == invoiceId && x.InvoiceNumber == invoiceNumber);

                if (dbInvoice == null)
                    return null;

                var invoice = new InvoicePrintModel
                {
                    Id = dbInvoice.Id,
                    InvoiceNumber = dbInvoice.InvoiceNumber,
                    InvoiceDate = dbInvoice.InvoiceDate,
                    CustomerName = dbInvoice.CustomerName,
                    MobileNo = dbInvoice.MobileNo,
                    PaymentMethod = dbInvoice.PaymentMethod,
                    GrossAmount = dbInvoice.GrossAmount,
                    DiscountPercent = dbInvoice.DiscountPercent,
                    DiscountAmount = dbInvoice.DiscountAmount,
                    TotalAmount = dbInvoice.TotalAmount
                };

                // Get Shop print data
                var printData = await _context.tblprintdata
                    .FirstOrDefaultAsync(p => p.ShopId == dbInvoice.shopid);

                if (printData != null)
                {
                    invoice.ShopName = printData.StoreName;
                    invoice.PrintMobileno = printData.MobileNo;
                    invoice.GstNumber = printData.GSTNumber;
                    invoice.PrintAddress = printData.Address;
                }
                else
                {
                    // Default values
                    var shop = await _context.tblShop.FirstOrDefaultAsync(s => s.Id == dbInvoice.shopid);
                    invoice.ShopName = shop?.ShopName ?? "Sri Sai Store";
                    invoice.PrintMobileno = shop?.ContactNumber ?? "+91 0000000000";
                    invoice.GstNumber = "20XXXXXXXXXX";
                    invoice.PrintAddress = "Main Road, Jamshedpur";
                }

                invoice.Items = await (
                    from s in _context.tblSaleDetailsInvoice
                    join p in _context.tblProduct
                        on s.ProductId equals p.Id
                    join u in _context.tblUnit
                        on p.UnitId equals u.Id
                    where s.InvoiceId == invoiceId
                    select new InvoiceItemModel
                    {
                        ProductId = p.Id,
                        ProductName = p.ProductName,
                        UnitName = u.UnitName,
                        Quantity = s.Quantity,
                        Rate = s.Rate,
                        Amount = s.Amount
                    }).ToListAsync();

                return invoice;
            }
            catch
            {
                throw;
            }
        }
        public ShopSettingModel GetPrintDataById(int id)
        {
            try
            {
                var ab= (from d in _context.tblprintdata
                        where d.Id == id
                        select new ShopSettingModel
                        {
                            Id = d.Id,
                            StoreName = d.StoreName,
                            GSTNumber = d.GSTNumber,
                            MobileNo = d.MobileNo,
                            Address = d.Address,
                            ShopId = d.ShopId
                        }).FirstOrDefault();
                return ab;
            }
            catch (Exception)
            {

                return new ShopSettingModel();
            }
            
        }
        public StaffModel GetStaffDetails(string? name)
        {
            try
            {
                var ab = (from d in _context.tblStaff
                          where d.StaffCode == name
                          select new StaffModel
                          {
                              Id= d.Id,
                              Shopid= d.Shopid,
                              Name=d.Name
                          }).FirstOrDefault();
                return ab;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AllResponseMessage> PostSaveBillData(ShopSettingModel ssm)
        {
            AllResponseMessage resp = new AllResponseMessage();
            try
            {
                EShopSettingModel? esm = null;
                if (ssm.Id > 0)
                {
                    esm = await _context.tblprintdata.FindAsync(ssm.Id);
                }
                else
                {
                    esm = await _context.tblprintdata.FirstOrDefaultAsync(x => x.ShopId == ssm.ShopId);
                }

                if (esm != null)
                {
                    esm.ShopId = ssm.ShopId;
                    esm.Address = ssm.Address;
                    esm.StoreName = ssm.StoreName;
                    esm.GSTNumber = ssm.GSTNumber;
                    esm.MobileNo = ssm.MobileNo;
                    _context.tblprintdata.Update(esm);
                }
                else
                {
                    esm = new EShopSettingModel
                    {
                        ShopId = ssm.ShopId,
                        Address = ssm.Address,
                        StoreName = ssm.StoreName,
                        GSTNumber = ssm.GSTNumber,
                        MobileNo = ssm.MobileNo
                    };
                    _context.tblprintdata.Add(esm);
                }
                await _context.SaveChangesAsync();

                resp.Result = true;
                resp.Message = "Data Save Successfully";
                return resp;

            }
            catch (Exception)
            {

                return new AllResponseMessage();
            }
        }

        public List<ShopSettingModel> GetAllPrintDataContent()
        {
            try
            {
                var ab = (from d in _context.tblprintdata
                          join sh in _context.tblShop on d.ShopId equals sh.Id
                          select new ShopSettingModel
                          {
                              GSTNumber=d.GSTNumber,
                              MobileNo=d.MobileNo,
                              StoreName=d.StoreName,
                              Address=d.Address,
                              Id=d.Id,
                              ShopName=sh.ShopName,
                          }).ToList();
                return ab;
            }
            catch (Exception)
            {

                return new List<ShopSettingModel>();
            }
        }


        public async Task<List<ShopSaleListVM>> GetAllItemShopSaleList(int shopid)
        {
            return await (
                from d in _context.tblSaleInvoice
                join ds in _context.tblSaleDetailsInvoice on d.Id equals ds.InvoiceId
                join p in _context.tblProduct on ds.ProductId equals p.Id
                join c in _context.tblCategory on p.CategoryId equals c.Id
                join u in _context.tblUnit on p.UnitId equals u.Id
                join s in _context.tblShop on d.shopid equals s.Id
                where d.shopid == shopid
                select new ShopSaleListVM
                {
                    InvoiceNumber = d.InvoiceNumber,
                    InvoiceDate = d.InvoiceDate,
                    ShopName = s.ShopName,
                    GrossAmount = d.GrossAmount,
                    CustomerName = d.CustomerName,
                    MobileNo = d.MobileNo,
                    DiscountPercent = d.DiscountPercent,
                    DiscountAmount = d.DiscountAmount,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Amount = ds.Amount,
                    Quantity = ds.Quantity,
                    TotalAmount = d.TotalAmount,
                    CategoryName = c.CategoryName,
                    ShortName = u.ShortName
                }).ToListAsync();
        }


        public (string Mobilno, string Gstnumber, string address, string shopname) GetPrintDataShopDetails(string? name)
        {
            try
            {
                int id = (from d in _context.tblStaff where d.StaffCode == name select d.Shopid).FirstOrDefault();
                if (id > 0)
                {
                    var ab = (from d in _context.tblprintdata
                              where d.ShopId == id
                              select new
                              {
                                  d.StoreName,
                                  d.MobileNo,
                                  d.GSTNumber,
                                  d.Address
                              }).FirstOrDefault();
                    if (ab != null)
                    {
                        return (ab.MobileNo, ab.GSTNumber, ab.Address, ab.StoreName);
                    }
                    else
                    {
                        var shop = _context.tblShop.FirstOrDefault(s => s.Id == id);
                        if (shop != null)
                        {
                            return (shop.ContactNumber ?? "+91 0000000000", "20XXXXXXXXXX", "Main Road, Jamshedpur", shop.ShopName);
                        }
                        return ("+91 0000000000", "20XXXXXXXXXX", "Main Road, Jamshedpur", "Sri Sai Store");
                    }
                }
                else
                {
                    return ("+91 0000000000", "20XXXXXXXXXX", "Main Road, Jamshedpur", "Sri Sai Store");
                }
                
            }
            catch (Exception)
            {

                return ("", "", "", "");
            }
        }


        public string GetGenerateBillNo()
        {
            try
            {
                // 1. Database se sabse latest invoice number nikalen
                var lastInvoice = _context.tblSaleInvoice
                                          .OrderByDescending(d => d.InvoiceNumber)
                                          .Select(d => d.InvoiceNumber)
                                          .FirstOrDefault();

                if (lastInvoice != null && lastInvoice.StartsWith("INV"))
                {
                    // 2. "INV" ke baad ka jo number hai (digits) usko extract karein
                    string numericPart = lastInvoice.Substring(3); // e.g., "20260601090320"

                    if (long.TryParse(numericPart, out long lastNo))
                    {
                        // 3. Pichle wale se ek jada (+1)
                        long nextNo = lastNo + 1;
                        return "INV" + nextNo.ToString();
                    }
                }

                // 4. Agar database khali hai (First Invoice), toh current timestamp se shuru karein
                return "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AllResponseMessage> ShopToWarehouseTransferAsync(ShopToWarehouseTransferRequest request)
        {
            var response = new AllResponseMessage();

            if (request == null || request.Items == null || !request.Items.Any())
            {
                response.Result = false;
                response.Message = "Koi item select nahi kiya gaya.";
                return response;
            }

            if (request.FromShopId == null || request.FromShopId == 0)
            {
                response.Result = false;
                response.Message = "Source shop select karein.";
                return response;
            }

            if (request.ToWarehouseId == null)
            {
                response.Result = false;
                response.Message = "Destination warehouse select karein.";
                return response;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int shopId = request.FromShopId.Value;
                int warehouseId = request.ToWarehouseId.Value;
                bool isAdminTransfer = warehouseId == 0;

                foreach (var item in request.Items)
                {
                    if (item.Quantity <= 0)
                    {
                        response.Result = false;
                        response.Message = $"Invalid quantity for ProductId {item.ProductId}.";
                        await transaction.RollbackAsync();
                        return response;
                    }

                    // 1. Fetch current shop stock row
                    var shopStock = await _context.tblshopstock
                        .FirstOrDefaultAsync(x => x.ShopId == shopId && x.ProductId == item.ProductId);

                    if (shopStock == null || shopStock.Quantity < item.Quantity)
                    {
                        response.Result = false;
                        response.Message = $"ProductId {item.ProductId} ka stock shop me kaafi nahi hai.";
                        await transaction.RollbackAsync();
                        return response;
                    }

                    // 2. Deduct from shop stock
                    shopStock.Quantity -= item.Quantity;
                    _context.tblshopstock.Update(shopStock);

                    decimal previousQty;
                    decimal newQty;

                    // 3. Add stock — Admin (tblStock) ya real Warehouse (EWarehousestock)
                    if (isAdminTransfer)
                    {
                        var adminStock = await _context.tblstock
                            .FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                        if (adminStock == null)
                        {
                            previousQty = 0;
                            newQty = item.Quantity;

                            _context.tblstock.Add(new EStock
                            {
                                ProductId = item.ProductId,
                                Quantity = newQty
                            });
                        }
                        else
                        {
                            previousQty = adminStock.Quantity;
                            newQty = previousQty + item.Quantity;

                            adminStock.Quantity = newQty;
                            _context.tblstock.Update(adminStock);

                            EStockHistory es = new EStockHistory()
                            {
                                StockId=adminStock.Id,
                                Quantity=item.Quantity,
                                EntryDate=DateTime.UtcNow,
                                Type="In",
                                Remark="Stock Ajust come from the Shop whose"
                            };

                            _context.tblWarehousestock.Add(new EWarehousestock
                            {
                                WarehouseId = warehouseId,
                                ProductId = item.ProductId,
                                Quantity = newQty,
                                LastUpdated = DateTime.Now
                            });
                        }
                    }
                    else
                    {
                        var whStock = await _context.tblWarehousestock
                            .FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ProductId == item.ProductId);

                        if (whStock == null)
                        {
                            previousQty = 0;
                            newQty = item.Quantity;

                            _context.tblWarehousestock.Add(new EWarehousestock
                            {
                                WarehouseId = warehouseId,
                                ProductId = item.ProductId,
                                Quantity = newQty,
                                LastUpdated = DateTime.Now
                            });
                        }
                        else
                        {
                            previousQty = whStock.Quantity;
                            newQty = previousQty + item.Quantity;

                            whStock.Quantity = newQty;
                            whStock.LastUpdated = DateTime.Now;
                            _context.tblWarehousestock.Update(whStock);
                        }
                    }

                    // 4. History log — actual EWarehouseStockHistory structure ke hisaab se
                    _context.tblWarehousestockhistory.Add(new EWarehouseStockHistory
                    {
                        WarehouseId = warehouseId, // admin transfer ke liye ye 0 rahega
                        ProductId = item.ProductId,
                        QuantityChanged = item.Quantity,   // positive — IN
                        PreviousQuantity = previousQty,
                        NewQuantity = newQty,
                        ReferenceId = null,                // agar transfer log table hai toh uska Id yahan daal sakte ho
                        ReferenceType = "ShopToWarehouseTransfer",
                        RelatedShopId = shopId,
                        Remarks = isAdminTransfer
                            ? $"Received in Admin stock from Shop #{shopId}"
                            : $"Received in Warehouse #{warehouseId} from Shop #{shopId}",
                        TransactionDate = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                response.Result = true;
                response.Message = "Stock transfer successfully ho gaya.";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                response.Result = false;
                response.Message = "Transfer save karte waqt error aaya.";
            }

            return response;
        }

        public async Task<List<StockAdjustmentListDto>> GetAllStockAdjustmentList(string warehousecode, DateTime? fromDate, DateTime? toDate)
        {
            // Sirf shop-to-warehouse (Stock Adjustment) movements chahiye, koi aur type nahi
            IQueryable<EWarehouseStockHistory> query = _context.tblWarehousestockhistory
                .Where(h => h.ReferenceType == "ShopToWarehouseTransfer");

            bool isAdmin = string.Equals(warehousecode, "Admin", StringComparison.OrdinalIgnoreCase);

            //if (!isAdmin)
            //{
            //    // Warehouse user apna hi data dekhega
            //    if (int.TryParse(warehousecode, out int warehouseId))
            //    {
            //        query = query.Where(h => h.WarehouseId == warehouseId);
            //    }
            //    else
            //    {
            //        // warehousecode numeric nahi hai — agar tumhare paas tblWarehouse (Code -> Id mapping)
            //        // table hai, toh yahan lookup karo. Filhaal agar match nahi mila toh khaali list return hogi.
            //        // Example:
            //        // var warehouse = await _context.tblWarehouse
            //        //     .FirstOrDefaultAsync(w => w.WarehouseCode == warehousecode);
            //        // if (warehouse != null)
            //        //     query = query.Where(h => h.WarehouseId == warehouse.Id);
            //        // else
            //        //     return new List<StockAdjustmentListDto>();

            //        return new List<StockAdjustmentListDto>();
            //    }
            //}
            // isAdmin == true => koi WarehouseId filter nahi, sab dikhega (Admin ka apna WarehouseId=0 bhi included)

            if (fromDate.HasValue)
                query = query.Where(h => h.TransactionDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(h => h.TransactionDate.Date <= toDate.Value.Date);

            var result = await (from h in query
                                join p in _context.tblProduct on h.ProductId equals p.Id
                                orderby h.TransactionDate descending
                                
                                select new StockAdjustmentListDto
                                {
                                    Id = h.Id,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    WarehouseId = h.WarehouseId,
                                    Warehousename=_context.tblwarehouse.Where(x=>x.Id==h.WarehouseId).Select(z=>z.WarehouseName).FirstOrDefault()??"Admin",
                                    QuantityChanged = h.QuantityChanged,
                                    PreviousQuantity = h.PreviousQuantity,
                                    NewQuantity = h.NewQuantity,
                                    RelatedShopId = h.RelatedShopId,
                                    RelatedShopName = _context.tblShop.Where(x=>x.Id==h.RelatedShopId).Select(z=>z.ShopName).FirstOrDefault()??"#Shop",
                                    Remarks = h.Remarks,
                                    TransactionDate = h.TransactionDate
                                }).ToListAsync();

            return result;
        }

        public async Task<AllResponseMessage> CreateBulkOrderAsync(BulkOrderModel model, int shopid, int staffid)
        {
            var resp = new AllResponseMessage();
            try
            {
                var order = new EBulkOrder
                {
                    OrderNumber = "BLK-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    CustomerName = model.CustomerName,
                    ContactNumber = model.ContactNumber,
                    DeliveryDate = model.DeliveryDate,
                    AdvanceAmount = model.AdvanceAmount,
                    TotalAmount = model.TotalAmount,
                    PaymentMethod = model.PaymentMethod ?? "Cash",
                    Remarks = model.Remarks,
                    ShopId = shopid,
                    StaffId = staffid,
                    Status = "Pending",
                    OrderDate = DateTime.Now,
                    IsDeleted = false
                };

                _context.tblBulkOrder.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in model.Items)
                {
                    var detail = new EBulkOrderDetail
                    {
                        BulkOrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Rate = item.Rate,
                        Amount = item.Amount
                    };
                    _context.tblBulkOrderDetail.Add(detail);
                }

                await _context.SaveChangesAsync();
                resp.Result = true;
                resp.Message = "Bulk Order created successfully with Order Number: " + order.OrderNumber;
                return resp;
            }
            catch (Exception ex)
            {
                resp.Result = false;
                resp.Message = "Failed to create bulk order: " + ex.Message;
                return resp;
            }
        }

        public async Task<List<BulkOrderModel>> GetAllBulkOrdersAsync()
        {
            try
            {
                var query = from bo in _context.tblBulkOrder
                            join sh in _context.tblShop on bo.ShopId equals sh.Id into shJoin
                            from sh in shJoin.DefaultIfEmpty()
                            join st in _context.tblStaff on bo.StaffId equals st.Id into stJoin
                            from st in stJoin.DefaultIfEmpty()
                            select new BulkOrderModel
                            {
                                Id = bo.Id,
                                OrderNumber = bo.OrderNumber,
                                CustomerName = bo.CustomerName,
                                ContactNumber = bo.ContactNumber,
                                OrderDate = bo.OrderDate,
                                DeliveryDate = bo.DeliveryDate,
                                Status = bo.Status,
                                TotalAmount = bo.TotalAmount,
                                AdvanceAmount = bo.AdvanceAmount,
                                PaymentMethod = bo.PaymentMethod,
                                Remarks = bo.Remarks,
                                ShopId = bo.ShopId,
                                ShopName = sh != null ? sh.ShopName : "Unknown Shop",
                                StaffName = st != null ? st.Name : "Unknown Staff"
                            };

                return await query.OrderByDescending(x => x.OrderDate).ToListAsync();
            }
            catch (Exception)
            {
                return new List<BulkOrderModel>();
            }
        }
    }
}
