using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POSDb.EntityModels;
using POSDb.EntityModels.MithaiShop;
using POSDb.EntityModels.POSModels.Models;
using POSDb.EntityModels.PurchaseEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔥 Identity configuration ke liye MUST
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EShop>()
                .HasOne(s => s.Warehouse)
                .WithMany(w => w.Shops)
                .HasForeignKey(s => s.WarehouseId);
            
            modelBuilder.Entity<EPurchaseDetail>()
                .HasOne(x => x.Purchase)
                .WithMany(x => x.PurchaseDetails)
                .HasForeignKey(x => x.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<EWarehouseModel> tblwarehouse { get; set; }
        public DbSet<EShop> tblShop { get; set; }
        public DbSet<ECategoryModel> tblCategory { get; set; }
        public DbSet<EBaseModel> tblProductBase { get; set; }
        public DbSet<EUnit> tblUnit { get; set; }
        public DbSet<EProduct> tblProduct { get; set; }
        public DbSet<EProductUnit> tblProductUnit { get; set; }
        public DbSet<EProduction> tblProduction { get; set; }
        public DbSet<EPurchaseProductModel> tblpurchaseproduct { get; set; }

        public DbSet<EpurchaseEntry> tblpurchaseentry { get; set; }
        public DbSet<EPurchaseDetail> tblpurchasedetail { get; set; }
        public DbSet<EStaffModel> tblStaff { get; set; }
        public DbSet<EStock> tblstock { get; set; }
        public DbSet<EStockHistory> tblStockHistory { get; set;  }
        public DbSet<ECart> tblCart { get; set; }
        public DbSet<ESalesInvoice> tblSaleInvoice { get; set; }
        public DbSet<ESalesInvoiceDetail> tblSaleDetailsInvoice { get; set; }
        public DbSet<EShopSettingModel> tblprintdata { get; set; }
        public DbSet<EStockTransfer> tblStockTransfer { get; set; }
        public DbSet<EStockTransferDetail> tblStockTransferDetails { get; set; }
        public DbSet<EShopStock> tblshopstock { get; set;  }
        public DbSet<EShopStockHistory> tblshopstockHistory { get; set; }
    }
}
