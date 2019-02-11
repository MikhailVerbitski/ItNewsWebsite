using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels.User;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user, admin")]
    [ApiController]
    [Route("api/User/[action]")]
    public class UserController : Controller
    {
        private readonly ServiceOfSection serviceOfSection;
        private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfTag serviceOfTag;
        private readonly ServiceOfPost serviceOfPost;
        private readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfAccount serviceOfAccount;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfRole serviceOfRole;

        public UserController(
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<RoleEntity> roleManager,
            ApplicationDbContext context,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment
            )
        {
            serviceOfRole = new ServiceOfRole(context, userManager, roleManager, mapper);
            serviceOfTag = new ServiceOfTag(context, mapper);
            serviceOfSection = new ServiceOfSection(context, mapper);
            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfAccount = new ServiceOfAccount(context, userManager, roleManager, hostingEnvironment, mapper);
            serviceOfComment = new ServiceOfComment(context, mapper, serviceOfUser);
            serviceOfPost = new ServiceOfPost(context, mapper, serviceOfImage, serviceOfAccount, serviceOfComment, serviceOfUser, serviceOfTag, serviceOfRole);
            serviceOfUser = new ServiceOfUser(context, userManager, mapper, serviceOfImage, serviceOfAccount, serviceOfComment, serviceOfPost, serviceOfRole);

            serviceOfComment.serviceOfUser = serviceOfUser;
            serviceOfPost.serviceOfUser = serviceOfUser;
            serviceOfPost.serviceOfUser = serviceOfUser;
            serviceOfRole.serviceOfUser = serviceOfUser;
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
            var path = serviceOfUser.ChangeUserImage(currentUserId, image);
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
