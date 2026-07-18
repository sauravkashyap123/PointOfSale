using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class LoginModel
    {
        public int Id { get; set; } = 0;
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Pin { get; set; }
        public string Role { get; set; }
        public bool? RememberMe { get; set; }
    }
}
