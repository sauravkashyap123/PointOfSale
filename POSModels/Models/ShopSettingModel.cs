using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class ShopSettingModel
    {
        public int Id { get; set; }

        public string? StoreName { get; set; }
        public string? ShopName { get; set; }

        public string? GSTNumber { get; set; }

        public string? MobileNo { get; set; }

        public string? Address { get; set; }

        public int ShopId { get; set; }
        public List<SelectListItem>? selectshoplist { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
