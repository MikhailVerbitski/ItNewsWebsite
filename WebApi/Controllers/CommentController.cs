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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user")]
    [ApiController]
    [Route("api/Comment/[action]")]
    public class CommentController : Controller
    {
        UserManager<ApplicationUserEntity> userManager;
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
            serviceOfComment = new ServiceOfComment(context, roleManager, userManager, hostingEnvironment, mapper);
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
