// Hubs/OrderHub.cs
using Microsoft.AspNetCore.SignalR;

namespace FoodShopBlazor.Hubs
{
    public class OrderHub : Hub
    {
        public async Task NewOrderCreated(int orderId)
        {
            await Clients.All.SendAsync("NewOrderReceived", orderId);
        }

        public async Task UpdateOrderStatus(int orderId, string status)
        {
            await Clients.All.SendAsync("UpdateOrderStatus", orderId, status);
        }
    }
}
