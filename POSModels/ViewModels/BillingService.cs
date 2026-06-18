using Microsoft.EntityFrameworkCore;
using POSDb.Data;
using POSDb.EntityModels;
using POSDb.EntityModels.MithaiShop;
using POSModels.Models;
using POSModels.Models.MithaiShop;
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

        public async Task<InvoicePrintModel?> GetInvoiceAllDetailsById(int invoiceId,string invoiceNumber)
        {
            try
            {
                var invoice = await _context.tblSaleInvoice
                    .Where(x => x.Id == invoiceId &&
                                x.InvoiceNumber == invoiceNumber)
                    .Select(x => new InvoicePrintModel
                    {
                        Id = x.Id,
                        InvoiceNumber = x.InvoiceNumber,
                        InvoiceDate = x.InvoiceDate,
                        CustomerName = x.CustomerName,
                        MobileNo = x.MobileNo,
                        PaymentMethod = x.PaymentMethod,
                        GrossAmount = x.GrossAmount,
                        DiscountPercent = x.DiscountPercent,
                        DiscountAmount = x.DiscountAmount,
                        TotalAmount = x.TotalAmount
                    })
                    .FirstOrDefaultAsync();

                if (invoice == null)
                    return null;

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
                EShopSettingModel esm = new EShopSettingModel();
                esm.ShopId = ssm.ShopId;
                esm.Address = ssm.Address;
                esm.StoreName = ssm.StoreName;
                esm.GSTNumber = ssm.GSTNumber;
                esm.MobileNo = ssm.MobileNo;
                 _context.tblprintdata.Add(esm);
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
                        return ("", "", "", "");
                    }
                }
                else
                {
                    return ("", "", "", "");
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
    }
}
