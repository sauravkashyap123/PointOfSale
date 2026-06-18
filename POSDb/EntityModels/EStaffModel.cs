using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels
{
    public class EStaffModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mobileno { get; set; }
        public string? Address { get; set; }
        public string Password { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? DOJ { get; set; }
        public string? AadharCardNo { get; set; }
        public string StaffCode { get; set; }
        public string? Emailid { get; set; }
        public int Shopid { get; set; }
    }
}
