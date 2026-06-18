using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.MithaiShop
{
    public class Product:BaseModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchaseRate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SaleRate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GSTPercent { get; set; }

        public int UnitId { get; set; }
        public decimal StockQuantity { get; set; } = 0.00m;
        public string? UnitName { get; set; }

        public bool IsWeightMachine { get; set; } = false;
        public bool IsPieceWise { get; set; } = false;

        public int ExpiryDays { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }
        
        public string? Description { get; set; }
        public List<SelectListItem>? SelectUnitList { get; set; }
        public List<SelectListItem>? SelectedCategoryList { get; set; }

        public List<Product>? productList { get; set;  }
    }
}
