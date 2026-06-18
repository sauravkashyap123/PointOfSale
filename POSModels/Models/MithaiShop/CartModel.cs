using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.MithaiShop
{
    public class CartModel
    {
        public int Id { get; set; }

        public int StaffId { get; set; }
        public int Shopid { get; set; }
        public string ShopName { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }

        public decimal Amount { get; set; }

        public string UnitName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
