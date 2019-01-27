using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/Post/[action]")]
    public class PostController : Controller
    {
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
            serviceOfPost = new ServiceOfPost(context, roleManager, userManager, hostingEnvironment, mapper);
            serviceOfSection = new ServiceOfSection(context, mapper);
            serviceOfTag = new ServiceOfTag(context, mapper);
        }

        //
        string temporaryUserId = "e46bc008-f20e-4a2b-b9ed-025135801130";
        //

        [HttpPost]
        public void CreatePost([FromBody] PostCreateEditViewModel postCreateEditViewModel)
        {
            var userId = temporaryUserId;//userManager.GetUserId(User);
            serviceOfPost.Create(userId, postCreateEditViewModel);
        }
        [HttpGet]
        public JsonResult PutEstimate(int postId, byte score)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var rating = serviceOfPost.RatingPost(currentUserId, postId, score);
            return Json(rating);
        }
        [HttpGet]
        public JsonResult PostViewModel(int postId)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var postViewModel = serviceOfPost.Get<PostViewModel>(currentUserId, postId);
            return Json(postViewModel);
        }
        [HttpGet]
        public JsonResult ListPostsCompactViewModel(int? take)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var posts = serviceOfPost.Get<PostCompactViewModel>(currentUserId, take);
            return Json(posts);
        }
        [HttpGet]
        public JsonResult ListPostsMiniViewModel(int? take)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var posts = serviceOfPost.Get<PostMiniViewModel>(currentUserId, take);
            return Json(posts);
        }
        [HttpGet]
        public JsonResult ListPostsCompactViewModelByTag(int tagId, int? take)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var posts = serviceOfPost.Get<PostCompactViewModel>(currentUserId, take, a => a.Tags.Any(b => b.TagId == tagId));
            return Json(posts);
        }
        [HttpGet]
        public JsonResult TheFirstSeveralWithTheHighestRating(int? take)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var posts = serviceOfPost.Get<PostMiniViewModel>(currentUserId, take, a => -((a.CountOfScore == 0) ? 0 : (int)(a.SumOfScore / a.CountOfScore)));
            return Json(posts);
        }
        [HttpGet]
        public JsonResult ListPostsCompactViewModelTop(int? take)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var posts = serviceOfPost.Get<PostCompactViewModel>(currentUserId, take, a => -((a.CountOfScore == 0) ? 0 : (int)(a.SumOfScore / a.CountOfScore)));
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
    }
}
