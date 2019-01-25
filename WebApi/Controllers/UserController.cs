using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : Controller
    {
        UserManager<ApplicationUserEntity> userManager;
        //private readonly IMapper mapper;
        //private readonly IHostingEnvironment hostingEnvironment;

        //private readonly ServiceOfPost serviceOfPost;
        //private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfSection serviceOfSection;
        //private readonly RoleManager<IdentityRole> roleManager;
        private readonly ServiceOfUser serviceOfUser;
        //private readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfTag serviceOfTag;

        public UserController(
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment
            )
        {
            this.userManager = userManager;
            //this.mapper = mapper;
            //this.hostingEnvironment = hostingEnvironment;
            //this.roleManager = roleManager;

            //serviceOfPost = new ServiceOfPost(context, roleManager, userManager, hostingEnvironment, mapper);
            //serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfSection = new ServiceOfSection(context, mapper);
            serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            //serviceOfComment = new ServiceOfComment(context, mapper);
            serviceOfTag = new ServiceOfTag(context, mapper);
        }


        [HttpGet("/api/User/GetUserByProperty")]
        public JsonResult GetUserByProperty(string property)
        {
            var users = serviceOfUser.GetUserByProperty(property);
            return Json(users);
        }
        [HttpGet("/api/User/GetUserViewModel")]
        public JsonResult GetUserViewModel(string login)
        {
            var user = serviceOfUser.GetUserViewModel(login);
            return Json(user);
        }
    }
}
