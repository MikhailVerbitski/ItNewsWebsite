using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user, admin")]
    [ApiController]
    [Route("api/Comment/[action]")]
    public class CommentController : Controller
    {
        public  readonly ServiceOfComment serviceOfComment;

        public CommentController(ServiceOfComment serviceOfComment)
        {
            this.serviceOfComment = serviceOfComment;
        }
        [HttpPost]
        public async Task<JsonResult> CreateComment([FromBody] CommentCreateEditViewModel commentViewModel)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var newComment = await serviceOfComment.Create(userId, commentViewModel);
            return Json(newComment);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int commentId)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            await serviceOfComment.Delete(userId, commentId);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> LikeComment(int commentId, int postId)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            await serviceOfComment.LikeComment(userId, commentId);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> DislikeComment(int commentId, int postId)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            await serviceOfComment.DislikeComment(userId, commentId);
            return Ok();
        }
    }
}
