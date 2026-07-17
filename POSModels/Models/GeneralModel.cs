using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models
{
    public class GeneralModel
    {
        public int TotalShop { get;  set; }
        public int TotalWarehouse { get;  set; }
        public int TotalProduct { get;  set; }
        public int TotalStaff { get;  set; }
        public int TotalCategory { get;  set; }

        // Dynamic Dashboard Properties
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal TodaySales { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockAlerts { get; set; }
        public int ActiveShops { get; set; }
        public int ActiveUsers { get; set; }

        public List<RecentActivityModel> RecentActivities { get; set; } = new();
    }

    public class RecentActivityModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TimeAgo { get; set; }
        public string Icon { get; set; }
        public string IconClass { get; set; }
    }
}
