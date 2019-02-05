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

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user")]
    [ApiController]
    [Route("api/User/[action]")]
    public class UserController : Controller
    {
        private readonly ServiceOfSection serviceOfSection;
        private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfTag serviceOfTag;

        public UserController(
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment
            )
        {
            serviceOfSection = new ServiceOfSection(context, mapper);
            serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            serviceOfTag = new ServiceOfTag(context, mapper);
        }

        [HttpGet]
        public JsonResult GetUserByProperty(string property)
        {
            var users = serviceOfUser.GetUserByProperty(property);
            return Json(users);
        }
        [HttpGet]
        public JsonResult GetUserViewModel(string login)
        {
            var user = serviceOfUser.GetUserViewModel(login);
            return Json(user);
        }
        [HttpPost]
        public void Update([FromBody] UserUpdateViewModel userUpdateViewModel)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            serviceOfUser.Update(userId, userUpdateViewModel);
        }
        [HttpPost]
        public JsonResult ChangeImage([FromBody] UserImage image)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var path = serviceOfUser.ChangeUserImage(currentUserId, image);
            return Json(path);
        }
    }
}
