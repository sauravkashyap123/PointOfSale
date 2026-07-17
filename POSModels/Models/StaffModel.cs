using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class StaffModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShopName { get; set; }
        public int Shopid { get; set; }
        public string Mobileno { get; set; }
        public string? Address { get; set; }
        public string Password { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? DOJ { get; set; }
        public string? AadharCardNo { get; set; }
        public string StaffCode { get; set; }
        public string? Email { get; set; }
        public List<SelectListItem>? selectedshoplist { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
