using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.MithaiShop
{
    public class EShopStock
    {
        [Key]
        public int Id { get; set; }
        public int ShopId { get; set;  }
        public int ProductId { get; set;  }
        [Column(TypeName ="decimal(18,2)")]
        public decimal Quantity { get; set; } = 0.00m;
    }
    public class EShopStockHistory
    {
        [Key]
        public int Id { get; set; }
        public int Shopstockid { get; set;  }
        public int ShopId { get; set;  }
        public int ProductId { get; set;  }
        public string? Type { get; set;  }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; } = 0.00m;
        public DateTime EntryDate { get; set;  }
    }
}
