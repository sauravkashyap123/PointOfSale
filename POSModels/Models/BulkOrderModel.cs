using System;
using System.Collections.Generic;

namespace POSModels.Models
{
    public class BulkOrderModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AdvanceAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string? Remarks { get; set; }
        public int ShopId { get; set; }
        public string? ShopName { get; set; }
        public string? StaffName { get; set; }
        public List<BulkOrderItemModel> Items { get; set; } = new();
    }

    public class BulkOrderItemModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
