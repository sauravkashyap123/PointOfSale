using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels
{
    public class EShopSettingModel
    {
        [Key]
        public int Id { get; set; }

        public string? StoreName { get; set; }

        public string? GSTNumber { get; set; }

        public string? MobileNo { get; set; }

        public string? Address { get; set; }

        public int ShopId { get; set; }
    }
}
