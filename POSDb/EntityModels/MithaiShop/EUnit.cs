using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.EntityModels.MithaiShop
{
    public class EUnit:EBaseModel
    {
        public string UnitName { get; set; }
        public string ShortName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
