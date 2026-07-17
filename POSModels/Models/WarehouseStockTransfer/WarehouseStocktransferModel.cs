using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Models.WarehouseStockTransfer
{
    public class WarehouseStocktransferModel
    {

    }
    public class StockTransferItemDto
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class ShopToWarehouseTransferRequest
    {
        public string TransferType { get; set; }   // "ShopToWarehouse"
        public int? FromShopId { get; set; }
        public int? ToWarehouseId { get; set; }
        public int? FromWarehouseId { get; set; }   // unused in this flow, kept for symmetry
        public int? ToShopId { get; set; }           // unused in this flow, kept for symmetry
        public List<StockTransferItemDto> Items { get; set; } = new();
    }

    public class ShopProductStockDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string UnitName { get; set; }
        public decimal AvailableQty { get; set; }
    }
    public class StockAdjustmentListDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int WarehouseId { get; set; }
        public decimal QuantityChanged { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal NewQuantity { get; set; }
        public int? RelatedShopId { get; set; }
        public string? Remarks { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Warehousename { get;  set; }
        public string? RelatedShopName { get;  set; }
    }
}
