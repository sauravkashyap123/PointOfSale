using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.MithaiShop
{
    public class ECategoryModel:EBaseModel
    {
        public string CategoryName { get; set; }
        public string? CategoryCode { get; set; }
        public string? Description { get; set; }
    }
}
