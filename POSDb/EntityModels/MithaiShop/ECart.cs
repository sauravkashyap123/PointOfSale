using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.MithaiShop
{
    public class ECart
    {
        [Key]
        public int Id { get; set; }

        public int StaffId { get; set; }
        public int Shopid { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string UnitName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
