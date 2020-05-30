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
    [Route("api/Message/[action]")]
    public class MessageController : Controller
    {
        private readonly ServiceOfMessage serviceOfMessage;

        public MessageController(ServiceOfMessage serviceOfMessage)
        {
            this.serviceOfMessage = serviceOfMessage;
        }

        [HttpPost]
        public JsonResult Read(int? skip, int? take, int chatId)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var chats = serviceOfMessage.Get(null, currentUserId, skip, take, chatId);
            return (chats.Count == 1) ? Json(chats.FirstOrDefault()) : Json(chats);
        }
        [HttpPost]
        public async Task<JsonResult> Create(MessageViewModel messageViewModel)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            messageViewModel = await serviceOfMessage.Create(currentUserId, messageViewModel);
            return Json(messageViewModel);
        }
        [HttpPost]
        public async Task<JsonResult> Update(MessageViewModel messageViewModel)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            messageViewModel = await serviceOfMessage.Update(currentUserId, messageViewModel);
            return Json(messageViewModel);
        }
    }
}