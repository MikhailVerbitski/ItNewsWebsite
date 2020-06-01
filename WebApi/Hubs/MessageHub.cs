using Domain.Contracts.Models.ViewModels.Message;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebApi.Hubs
{
    public class MessageHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public void Send(MessageViewModel Message)
        {
            Clients.All.SendAsync("createMessage", Microsoft.JSInterop.Json.Serialize(Message));
        }
        public void Delete(int MessageId)
        {
            Clients.All.SendAsync("deleteMessage", MessageId);
        }
        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
