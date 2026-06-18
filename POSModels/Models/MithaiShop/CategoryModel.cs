using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace POSModels.Models.MithaiShop
{
    public class CategoryModel:BaseModel
    {
        public string CategoryName { get; set; }

        [Remote(action: "CheckCategoryCode",
            controller: "Home",
            AdditionalFields = "Id",
            ErrorMessage = "Category Code already exists")]
        public string? CategoryCode { get; set; }
        public string? Description { get; set; }
    }
}
