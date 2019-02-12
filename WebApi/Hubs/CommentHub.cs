using Domain.Contracts.Models.ViewModels.Comment;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebApi.Hubs
{
    public class CommentHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public void Send(CommentViewModel comment)
        {
            Clients.All.SendAsync("createComment", comment);
        }
        public void Delete(int commentId)
        {
            Clients.All.SendAsync("deleteComment", commentId);
        }
        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
