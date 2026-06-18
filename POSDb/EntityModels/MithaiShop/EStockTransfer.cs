using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.MithaiShop
{
    public class EStockTransfer
    {
        public int Id { get; set; }

        public int FromShopId { get; set; } // On Issue time it become FromWarehouse toShop

        public int ToShopId { get; set; } // On Return time it become FromWarehouse to shop 

        public DateTime TransferDate { get; set; }

        public string TransactionType { get; set; }

        public string Remarks { get; set; }
    }
    public class EStockTransferDetail
    {
        public int Id { get; set; }

        public int TransferId { get; set; }

        public int ProductId { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal Quantity { get; set; }
        public DateTime CreatedDate { get; set;  }
    }
}
