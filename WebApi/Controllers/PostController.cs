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
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user")]
    [ApiController]
    [Route("api/Post/[action]")]
    public class PostController : Controller
    {
        private readonly UserManager<ApplicationUserEntity> userManager;
        private readonly ServiceOfSection serviceOfSection;
        private readonly ServiceOfTag serviceOfTag;
        private readonly ServiceOfPost serviceOfPost;
        private readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfAccount serviceOfAccount;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfUser serviceOfUser;

        public PostController(
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment
            )
        {
            this.userManager = userManager;
            
            serviceOfTag = new ServiceOfTag(context, mapper);
            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfSection = new ServiceOfSection(context, mapper);
            serviceOfAccount = new ServiceOfAccount(context, userManager, roleManager, hostingEnvironment, mapper);
            serviceOfComment = new ServiceOfComment(context, mapper, serviceOfUser);
            serviceOfPost = new ServiceOfPost(context, mapper, serviceOfImage, serviceOfAccount, serviceOfComment, serviceOfUser, serviceOfTag);
            serviceOfUser = new ServiceOfUser(context, mapper, serviceOfImage, serviceOfAccount, serviceOfComment, serviceOfPost);

            serviceOfComment.serviceOfUser = serviceOfUser;
            serviceOfPost.serviceOfUser = serviceOfUser;
        }
        
        [HttpPost]
        public async Task<JsonResult> Create(PostUpdateViewModel post)
        {
            var userId = User.Claims.SingleOrDefault(a => a.Type == "UserId").Value;
            post = await serviceOfPost.Create(userId, post);
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
        [HttpGet]
        public JsonResult PostViewModel(int postId)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var postViewModel = serviceOfPost.Get<PostViewModel>(currentUserId, postId);
            return Json(postViewModel);
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult ListPostsCompactViewModel(int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var posts = serviceOfPost.Get<PostCompactViewModel>(currentUserId, take, null, a => a.IsFinished == true);
            return Json(posts);
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult ListPostsMiniViewModel(int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var posts = serviceOfPost.Get<PostMiniViewModel>(currentUserId, take, null, a => a.IsFinished == true);
            return Json(posts);
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult ListPostsCompactViewModelByTag(int tagId, int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var posts = serviceOfPost.Get<PostCompactViewModel>(currentUserId, take, null, a => a.Tags.Any(b => b.TagId == tagId), a => a.IsFinished == true);
            return Json(posts);
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult TheFirstSeveralWithTheHighestRating(int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var posts = serviceOfPost.Get<PostMiniViewModel>(currentUserId, take, a => -((a.CountOfScore == 0) ? 0 : (int)(a.SumOfScore / a.CountOfScore)), a => a.IsFinished == true);
            return Json(posts);
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult ListPostsCompactViewModelTop(int? take)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            var posts = serviceOfPost.Get<PostCompactViewModel>(currentUserId, take, a => -((a.CountOfScore == 0) ? 0 : (int)(a.SumOfScore / a.CountOfScore)), a => a.IsFinished == true);
            return Json(posts);
        }
        [HttpGet]
        public JsonResult GetListOfSelections()
        {
            var listOfSelections = serviceOfSection.Get().Select(a => a.Text);
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
            var path = serviceOfPost.AddImage(currentUserId, image);
            return Json(path);
        }
    }
}
