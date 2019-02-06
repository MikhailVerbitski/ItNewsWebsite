using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user, admin")]
    [ApiController]
    [Route("api/Comment/[action]")]
    public class CommentController : Controller
    {
        public  readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfPost serviceOfPost;
        private readonly ServiceOfAccount serviceOfAccount;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfTag serviceOfTag;
        private readonly ServiceOfUser serviceOfUser;

        public CommentController(
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper
            )
        {
            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfAccount = new ServiceOfAccount(context, userManager, roleManager, hostingEnvironment, mapper);
            serviceOfComment = new ServiceOfComment(context, mapper, serviceOfUser);
            serviceOfPost = new ServiceOfPost(context, mapper, serviceOfImage, serviceOfAccount, serviceOfComment, serviceOfUser, serviceOfTag);
            serviceOfUser = new ServiceOfUser(context, mapper, serviceOfImage, serviceOfAccount, serviceOfComment, serviceOfPost);
            serviceOfTag = new ServiceOfTag(context, mapper);

            serviceOfComment.serviceOfUser = serviceOfUser;
            serviceOfPost.serviceOfUser = serviceOfUser;
            serviceOfPost.serviceOfTag = serviceOfTag;
            serviceOfUser.serviceOfPost = serviceOfPost;
        }
        [HttpPost]
        public JsonResult CreateComment([FromBody] CommentCreateEditViewModel commentViewModel)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var newComment = serviceOfComment.Create(userId, commentViewModel);
            return Json(newComment);
        }

        [HttpGet]
        public IActionResult LikeComment(int commentId, int postId)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            serviceOfComment.LikeComment(userId, commentId);
            return Ok();
        }
        [HttpGet]
        public IActionResult DislikeComment(int commentId, int postId)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            serviceOfComment.DislikeComment(userId, commentId);
            return Ok();
        }
    }
}
