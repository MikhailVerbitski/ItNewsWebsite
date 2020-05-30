using Domain.Contracts.Models.ViewModels.Message;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user, writer, admin")]
    [ApiController]
    [Route("api/Chat/[action]")]
    public class ChatController : Controller
    {
        private readonly ServiceOfChat serviceOfChat;

        public ChatController(ServiceOfChat serviceOfChat)
        {
            this.serviceOfChat = serviceOfChat;
        }
        
        [HttpPost]
        public JsonResult Read()
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var chats = serviceOfChat.Get(null, currentUserId, null, null);
            return (chats.Count == 1) ? Json(chats.FirstOrDefault()) : Json(chats);
        }
        [HttpPost]
        public async Task<JsonResult> Create(ChatRoomViewModel chatRoomViewModel)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            chatRoomViewModel = await serviceOfChat.Create(chatRoomViewModel, currentUserId, new string[] { });
            return Json(chatRoomViewModel);
        }
        [HttpPost]
        public async Task<JsonResult> Update(ChatRoomViewModel chatRoomViewModel)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            await serviceOfChat.Update(currentUserId, chatRoomViewModel);
            return Json(chatRoomViewModel);
        }
    }
}