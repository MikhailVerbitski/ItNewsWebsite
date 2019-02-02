using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels.Post;
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
    [Route("api/Post/[action]")]
    public class PostController : Controller
    {
        private readonly UserManager<ApplicationUserEntity> userManager;
        private readonly ServiceOfPost serviceOfPost;
        private readonly ServiceOfSection serviceOfSection;
        private readonly ServiceOfTag serviceOfTag;

        public PostController(
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment
            )
        {
            this.userManager = userManager;
            serviceOfPost = new ServiceOfPost(context, roleManager, userManager, hostingEnvironment, mapper);
            serviceOfSection = new ServiceOfSection(context, mapper);
            serviceOfTag = new ServiceOfTag(context, mapper);
        }
        

        [HttpPost]
        public JsonResult Create(PostUpdateViewModel post)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            post = serviceOfPost.Create(userId, post);
            return Json(post);
        }
        [HttpGet]
        public JsonResult Edit(int postId)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var post = serviceOfPost.Get<PostUpdateViewModel>(userId, postId);
            return Json(post);
        }
        [HttpPost]
        public void Update([FromBody] PostUpdateViewModel postCreateEditViewModel)
        {
            serviceOfPost.Update(postCreateEditViewModel);
        }
        [HttpGet]
        public JsonResult PutEstimate(int postId, byte score)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var rating = serviceOfPost.RatingPost(currentUserId, postId, score);
            return Json(rating);
        }
        [HttpGet]
        public JsonResult PostViewModel(int postId)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var postViewModel = serviceOfPost.Get<PostViewModel>(currentUserId, postId);
            return Json(postViewModel);
        }
        [HttpGet]
        public JsonResult ListPostsCompactViewModel(int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var posts = serviceOfPost.Get<PostCompactViewModel>(currentUserId, take, null, a => a.IsFinished == true);
            return Json(posts);
        }
        [HttpGet]
        public JsonResult ListPostsMiniViewModel(int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var posts = serviceOfPost.Get<PostMiniViewModel>(currentUserId, take, null, a => a.IsFinished == true);
            return Json(posts);
        }
        [HttpGet]
        public JsonResult ListPostsCompactViewModelByTag(int tagId, int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var posts = serviceOfPost.Get<PostCompactViewModel>(currentUserId, take, null, a => a.Tags.Any(b => b.TagId == tagId), a => a.IsFinished == true);
            return Json(posts);
        }
        [HttpGet]
        public JsonResult TheFirstSeveralWithTheHighestRating(int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var posts = serviceOfPost.Get<PostMiniViewModel>(currentUserId, take, a => -((a.CountOfScore == 0) ? 0 : (int)(a.SumOfScore / a.CountOfScore)), a => a.IsFinished == true);
            return Json(posts);
        }
        [HttpGet]
        public JsonResult ListPostsCompactViewModelTop(int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            var posts = serviceOfPost.Get<PostCompactViewModel>(currentUserId, take, a => -((a.CountOfScore == 0) ? 0 : (int)(a.SumOfScore / a.CountOfScore)), a => a.IsFinished == true);
            return Json(posts);
        }
        [HttpGet]
        public JsonResult GetListOfSelections()
        {
            var listOfSelections = serviceOfSection.Get().Select(a => a.Text);
            return Json(listOfSelections);
        }
        [HttpGet]
        public JsonResult GetListOfTags()
        {
            var listOfTags = serviceOfTag.Get();
            return Json(listOfTags);
        }

        [HttpPost]
        public JsonResult AddImage([FromBody] ImageViewModel image)
        {
            var path = serviceOfPost.AddImage(image);
            return Json(path);
        }
    }
}
