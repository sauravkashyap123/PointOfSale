using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.MithaiShop
{
    public class StockTransfer
    {
        public int Id { get; set; }

        public int FromShopId { get; set; }

        public int ToShopId { get; set; }
        

        public DateTime TransferDate { get; set; }

        public string? TransactionType { get; set; }

        public string? Remarks { get; set; }
        public List<SelectListItem>? selectproduct { get; set;  }
        public List<Product>? productlist { get; set;  }
        public List<SelectListItem>? selectshop { get; set;  }
        public List<SelectListItem>? SelectWarehouse { get; set; }
    }
    public class StockTransferDetail
    {
        public int Id { get; set; }

        public int TransferId { get; set; }

        public int ProductId { get; set; }

        public decimal Quantity { get; set; }
    }
    public class StockTransferVM
    {
        public int Id { get; set; }
        public int productid { get; set; }
        public int shopid { get; set; }
        public decimal quantity { get; set; }
        public int warehouseid { get; set; }
        public string? remarks { get; set; }
    }
}
