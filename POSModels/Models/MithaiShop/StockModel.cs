using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.MithaiShop
{
    public class StockModel
    {
        public int Id { get; set; }
        public int productId { get; set; }

        public decimal Quantity { get; set;  }
    }
    public class StockHistoryModel
    {
        public int Id { get; set;  }
        public int stockid { get; set;  }
        public decimal Quantity { get; set;  }
        public string Type { get; set; }
        public DateTime EntryDate { get; set;  }
        public DateTime UpdateDate { get; set;  }
    }
   
}
