using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace POSModels.Models
    {
        [Index(nameof(WarehouseCode), IsUnique = true)]
        public class EWarehouseModel
        {
            [Key]
            public int Id { get; set; }
            public string WarehouseCode { get; set; }
            public string WarehouseName { get; set; }
            public string? Password { get; set; }
            public string? Address { get; set; }
            public int NoOfShop { get; set; } = 5;
            public string? Description { get; set; }
            public string? ContactNumber { get; set; }
            public string? Email { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Pincode { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
            public List<EShop>? Shops { get; set; }
            
        }
       
    }

}
