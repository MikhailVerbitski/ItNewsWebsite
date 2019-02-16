using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels.User;
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
    [Route("api/User/[action]")]
    public class UserController : Controller
    {
        private readonly ServiceOfUser serviceOfUser;

        public UserController(ServiceOfUser serviceOfUser)
        {
            this.serviceOfUser = serviceOfUser;
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult Search(string property)
        {
            var users = serviceOfUser.Search(property);
            return Json(users);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetUserViewModel(int userProfileId)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var user = await serviceOfUser.GetUserViewModel(userId, userProfileId);
            return Json(user);
        }
        [HttpPost]
        public async Task Update([FromBody] UserViewModel userViewModel)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            await serviceOfUser.Update(userId, userViewModel);
        }
        [HttpPost]
        public JsonResult ChangeImage([FromBody] UserImage image)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var path = serviceOfUser.ChangeUserImage(host, currentUserId, image);
            return Json(path);
        }
        [HttpGet]
        public async Task<JsonResult> GetDataAboutCurrentUser()
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var data = await serviceOfUser.GetUserData(userId);
            return Json(data);
        }
    }
}
