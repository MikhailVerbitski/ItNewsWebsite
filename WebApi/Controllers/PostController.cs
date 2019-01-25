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
    [Route("api/Post")]
    [ApiController]
    public class PostController : Controller
    {
        UserManager<ApplicationUserEntity> userManager;
        //private readonly IMapper mapper;
        //private readonly IHostingEnvironment hostingEnvironment;

        private readonly ServiceOfPost serviceOfPost;
        //private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfSection serviceOfSection;
        //private readonly RoleManager<IdentityRole> roleManager;
        //private readonly ServiceOfUser serviceOfUser;
        //private readonly ServiceOfComment serviceOfComment;
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
            //this.mapper = mapper;
            //this.hostingEnvironment = hostingEnvironment;
            //this.roleManager = roleManager;

            serviceOfPost = new ServiceOfPost(context, roleManager, userManager, hostingEnvironment, mapper);
            //serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfSection = new ServiceOfSection(context, mapper);
            //serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            //serviceOfComment = new ServiceOfComment(context, mapper);
            serviceOfTag = new ServiceOfTag(context, mapper);
        }

        //
        string temporaryUserId = "e46bc008-f20e-4a2b-b9ed-025135801130";
        //
        //[HttpGet("[action]")]
        //[Route("CreatePost")]
        //public JsonResult CreatePost()
        //{
        //    var currentUserId = temporaryUserId;//userManager.GetUserId(User);
        //    var postCreateEditViewModel = serviceOfPost.CreateNotFinished(currentUserId);
        //    return Json(postCreateEditViewModel.PostId);
        //}
        [HttpPost]
        [Route("CreatePost")]
        public void CreatePost([FromBody] PostCreateEditViewModel postCreateEditViewModel/*, IFormFile[] images*/)
        {
            var userId = temporaryUserId;//
            serviceOfPost.Create(userId, postCreateEditViewModel);
            //serviceOfPost.AddImage(postCreateEditViewModel.PostId, images);
        }
        [HttpGet("[action]")]
        [Route("PutEstimate")]
        public JsonResult PutEstimate(int postId, byte score)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var rating = serviceOfPost.RatingPost(currentUserId, postId, score);
            return Json(rating);
        }
        [HttpGet("[action]")]
        [Route("PostViewModel")]
        public JsonResult PostViewModel(int postId)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var postViewModel = serviceOfPost.Get<PostViewModel>(currentUserId, postId);
            return Json(postViewModel);
        }
        [HttpGet("[action]")]
        [Route("ListPostsViewModel")]
        public JsonResult ListPostsViewModel()
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var posts = serviceOfPost.Get<PostViewModel>(currentUserId);
            return Json(posts);
        }
        [HttpGet("[action]")]
        [Route("ListPostsViewModelByTag")]
        public JsonResult ListPostsViewModelByTag(int tagId)
        {
            var currentUserId = temporaryUserId;//userManager.GetUserId(User);
            var posts = serviceOfPost.Get<PostViewModel>(currentUserId, a => a.Tags.Any(b => b.TagId == tagId));
            return Json(posts);
        }
        [HttpGet("[action]")]
        [Route("GetListOfSelections")]
        public JsonResult GetListOfSelections()
        {
            var listOfSelections = serviceOfSection.Get().Select(a => a.Text);
            return Json(listOfSelections);
        }
        [HttpGet("[action]")]
        [Route("api/Post/GetListOfTags")]
        public JsonResult GetListOfTags()
        {
            var listOfTags = serviceOfTag.Get();
            return Json(listOfTags);
        }
    }
}
