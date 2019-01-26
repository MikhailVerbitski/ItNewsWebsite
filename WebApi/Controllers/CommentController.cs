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

        //
        string temporaryUserId = "e46bc008-f20e-4a2b-b9ed-025135801130";
        //

        [HttpPost]
        public JsonResult CreateComment([FromBody] CommentCreateEditViewModel commentViewModel)
        {
            var user = temporaryUserId;
            var newComment = serviceOfComment.Create(user, commentViewModel);
            return Json(newComment);
        }

        [HttpGet]
        public IActionResult LikeComment(int commentId, int postId)
        {
            var user = temporaryUserId;
            serviceOfComment.LikeComment(user, commentId);
            return Ok();
        }
        [HttpGet]
        public IActionResult DislikeComment(int commentId, int postId)
        {
            var user = temporaryUserId;
            serviceOfComment.DislikeComment(user, commentId);
            return Ok();
        }
    }
}
