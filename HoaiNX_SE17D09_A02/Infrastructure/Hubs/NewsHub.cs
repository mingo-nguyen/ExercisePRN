using HoaiNXRazorPages.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HoaiNXRazorPages.Infrastructure.Hubs
{
    public class NewsHub : Hub
    {
        // These methods are called from JavaScript
        //public async Task CreateNewsArticle(object text)
        //{
        //    await Clients.All.SendAsync("ReceiveNewNewsArticle", text);
        //}
        //public async Task UpdateNewsArticle(object newsArticle)
        //{
        //    await Clients.All.SendAsync("ReceiveUpdatedNewsArticle", newsArticle);
        //}

        //public async Task DeleteNewsArticle(string newsArticleId)
        //{
        //    await Clients.All.SendAsync("ReceiveDeletedNewsArticle", newsArticleId);
        //}

        //public override async Task OnConnectedAsync()
        //{
        //    await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        //    await base.OnDisconnectedAsync(exception);
        //}
    }
}
