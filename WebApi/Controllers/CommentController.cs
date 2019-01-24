using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/Comment")]
    
    [ApiController]
    public class CommentController : Controller
    {
        UserManager<ApplicationUserEntity> userManager;
        //private readonly IMapper mapper;
        //private readonly IHostingEnvironment hostingEnvironment;

        //private readonly ServiceOfPost serviceOfPost;
        //private readonly ServiceOfImage serviceOfImage;
        //private readonly ServiceOfSection serviceOfSection;
        //private readonly RoleManager<IdentityRole> roleManager;
        //private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfComment serviceOfComment;

        public CommentController(
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper
            )
        {
            this.userManager = userManager;
            //this.mapper = mapper;
            //this.hostingEnvironment = hostingEnvironment;
            //this.roleManager = roleManager;

            //serviceOfPost = new ServiceOfPost(context, mapper, hostingEnvironment);
            //serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            //serviceOfSection = new ServiceOfSection(context, mapper);
            //serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            serviceOfComment = new ServiceOfComment(context, roleManager, userManager, hostingEnvironment, mapper);
        }

        //
        string temporaryUserId = "e46bc008-f20e-4a2b-b9ed-025135801130";
        //

        [HttpPost("/api/Comment/CreateComment")]
        public JsonResult CreateComment([FromBody] CommentCreateEditViewModel commentViewModel)
        {
            var user = temporaryUserId;
            var newComment = serviceOfComment.Create(user, commentViewModel);
            return Json(newComment);
        }

        [HttpGet("[action]")]
        [Route("api/commetn/LikeComment")]
        public IActionResult LikeComment(int commentId, int postId)
        {
            var user = temporaryUserId;
            serviceOfComment.LikeComment(user, commentId);
            return Ok();
        }
        [HttpGet("[action]")]
        [Route("api/Comment/DislikeComment")]
        public IActionResult DislikeComment(int commentId, int postId)
        {
            var user = temporaryUserId;
            serviceOfComment.DislikeComment(user, commentId);
            return Ok();
        }
    }
}
