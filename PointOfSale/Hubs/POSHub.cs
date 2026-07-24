using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PointOfSale.Hubs
{
    public class POSHub : Hub
    {
        // Broadcast new invoice/sale created to all cashier terminals & admin dashboards
        public async Task BroadcastNewInvoiceCreated(string invoiceNumber, decimal totalAmount, string customerName)
        {
            await Clients.All.SendAsync("ReceiveNewInvoiceNotification", new
            {
                InvoiceNumber = invoiceNumber,
                TotalAmount = totalAmount,
                CustomerName = customerName,
                Timestamp = System.DateTime.Now.ToString("g")
            });
        }

        // Broadcast stock transfer or stock updates
        public async Task BroadcastStockUpdated(int productId, string productName, int updatedStock)
        {
            await Clients.All.SendAsync("ReceiveStockUpdateNotification", new
            {
                ProductId = productId,
                ProductName = productName,
                Stock = updatedStock
            });
        }

        // Broadcast notification alerts
        public async Task SendNotification(string title, string message, string type = "info")
        {
            await Clients.All.SendAsync("ReceiveGeneralNotification", new { Title = title, Message = message, Type = type });
        }
    }
}
