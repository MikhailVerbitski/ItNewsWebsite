using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
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
    }
}
