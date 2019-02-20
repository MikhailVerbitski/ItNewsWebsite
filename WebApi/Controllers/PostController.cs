using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels.Post;
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
    [Route("api/Post/[action]")]
    public class PostController : Controller
    {
        private readonly ServiceOfSection serviceOfSection;
        private readonly ServiceOfTag serviceOfTag;
        private readonly ServiceOfPost serviceOfPost;

        public PostController(ServiceOfSection serviceOfSection, ServiceOfTag serviceOfTag, ServiceOfPost serviceOfPost )
        {
            this.serviceOfTag = serviceOfTag;
            this.serviceOfSection = serviceOfSection;
            this.serviceOfPost = serviceOfPost;
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult Search(string property)
        {
            var posts = serviceOfPost.Search(property);
            return Json(posts);
        }
        [HttpGet]
        public async Task<JsonResult> Create()
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var post = await serviceOfPost.Create(userId, null);
            return Json(post);
        }
        [HttpPost]
        public async Task<JsonResult> Create(PostUpdateViewModel post)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            post = await serviceOfPost.Create(userId, post);
            return Json(post);
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] PostUpdateViewModel postCreateEditViewModel)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            await serviceOfPost.Update(userId, postCreateEditViewModel);
            return Ok();
        }
        [HttpGet]
        public async Task Delete(int postId)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            await serviceOfPost.Delete(currentUserId, postId);
        }
        [HttpGet]
        public JsonResult PutEstimate(int postId, byte score)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var rating = serviceOfPost.RatingPost(currentUserId, postId, score);
            return Json(rating);
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Read([FromBody]PostReadRequestParams readRequest)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var post = serviceOfPost.Get(readRequest.type, currentUserId, readRequest.skip, readRequest.count, readRequest.where, readRequest.orderBy);
            if(readRequest.count != null && readRequest.count == 1)
            {
                return Json(post.FirstOrDefault());
            }
            return Json(post);
        }
        [HttpGet]
        public JsonResult GetListOfSelections()
        {
            var listOfSelections = serviceOfSection.Get();
            return Json(listOfSelections);
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetListOfTags()
        {
            var listOfTags = serviceOfTag.Get();
            return Json(listOfTags);
        }

        [HttpPost]
        public JsonResult AddImage([FromBody] PostImage image)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var path = serviceOfPost.AddImage(host, currentUserId, image);
            return Json(path);
        }
    }
}