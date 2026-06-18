using POSModels.Models.MithaiShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class AllResponseMessage
    {
        public string? Message { get; set; } = "Error";
        public string? Error { get; set; }
        public bool? Result { get; set; } = false;
        public List<CartModel>? cart { get; set;  }
        public Dictionary<int, string>? Extra { get; set; }
        public string? redirect { get;  set; }
    }
}
