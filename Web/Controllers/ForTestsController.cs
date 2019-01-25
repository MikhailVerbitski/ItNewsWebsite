using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Contracts.Models.ViewModels.User;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Web.Controllers
{
    //[Authorize(Roles = "user")]
    public class ForTestsController : Controller
    {
        UserManager<ApplicationUserEntity> userManager;
        private readonly IMapper mapper;
        private readonly IHostingEnvironment hostingEnvironment;

        private readonly ServiceOfPost serviceOfPost;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfSection serviceOfSection;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfComment serviceOfComment;

        public ForTestsController(
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context, 
            IMapper mapper, 
            IHostingEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;

            serviceOfPost = new ServiceOfPost(context, roleManager, userManager, hostingEnvironment, mapper);
            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfSection = new ServiceOfSection(context, mapper);
            serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            serviceOfComment = new ServiceOfComment(context, roleManager, userManager, hostingEnvironment, mapper);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreatePost()
        {
            ViewData["Sections"] = serviceOfSection.Get();
            var postId =  serviceOfPost.Create(userManager.GetUserId(User), null);
            var postCreateEditViewModel = serviceOfPost.Get<PostCreateEditViewModel>(userManager.GetUserId(User), postId);
            return View(postCreateEditViewModel);
        }
        [HttpPost]
        public IActionResult CreatePost(PostCreateEditViewModel postCreateEditViewModel, IFormFile[] images)
        {
            serviceOfPost.Update(postCreateEditViewModel, images);
            return View();
        }

        public IActionResult PostViewModel(int postId)
        {
            var postViewModel = serviceOfPost.Get<PostViewModel>(userManager.GetUserId(User), postId);
            return View(postViewModel);
        }
        public IActionResult PutEstimate(int postId, byte score)
        {
            serviceOfPost.RatingPost(userManager.GetUserId(User), postId, score);
            return RedirectToAction("PostViewModel", new { postId = postId });
        }

        [HttpPost]
        public IActionResult CreateComment(CommentCreateEditViewModel commentViewModel)
        {
            serviceOfComment.Create(userManager.GetUserId(User), commentViewModel);
            return RedirectToAction("PostViewModel", new { postId = commentViewModel.PostId });
        }
        public IActionResult LikeComment(int commentId, int postId)
        {
            serviceOfComment.LikeComment(userManager.GetUserId(User), commentId);
            return RedirectToAction("PostViewModel", new { postId = postId });
        }
        public IActionResult DislikeComment(int commentId, int postId)
        {
            serviceOfComment.DislikeComment(userManager.GetUserId(User), commentId);
            return RedirectToAction("PostViewModel", new { postId = postId });
        }

        public IActionResult ListPostsViewModel()
        {
            var start = Stopwatch.StartNew();
            var posts = serviceOfPost.Get<PostViewModel>(userManager.GetUserId(User));
            start.Stop();
            return View(posts);
        }
        public IActionResult ListNotFinishedPostsViewModel()
        {
            var posts = serviceOfPost.Get<PostViewModel>(userManager.GetUserId(User), a => a.IsFinished == false);
            return View(posts);
        }

        public IActionResult ListPostsMiniViewModel()
        {
            var posts = serviceOfPost.Get<PostMiniViewModel>(userManager.GetUserId(User));
            return View(posts);
        }
        public IActionResult ListNotFinishedPostsMiniViewModel()
        {
            var posts = serviceOfPost.Get<PostMiniViewModel>(userManager.GetUserId(User), a => a.IsFinished == false);
            return View(posts);
        }

        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> EditUser()
        {
            var userEditViewModel = await serviceOfUser.GetUserEditViewModel(userManager.GetUserId(User));
            return View(userEditViewModel);
        }
        [HttpPost]
        //[Authorize(Roles = "admin")]
        public IActionResult EditUser(UserEditViewModel userEditViewModel)
        {
            serviceOfUser.EditUser(userManager.GetUserId(User), userEditViewModel);
            return RedirectToAction("Index");
        }
    }
}