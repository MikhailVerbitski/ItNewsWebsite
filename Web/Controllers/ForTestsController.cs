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
using System.Collections.Generic;
using System.Linq;

namespace Web.Controllers
{
    [Authorize]
    public class ForTestsController : Controller
    {
        UserManager<ApplicationUserEntity> userManager;
        private readonly IMapper mapper;
        private readonly IHostingEnvironment hostingEnvironment;

        private readonly ServiceOfPost serviceOfPost;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfSection serviceOfSection;

        public ForTestsController(
            UserManager<ApplicationUserEntity> userManager, 
            ApplicationDbContext context, 
            IMapper mapper, 
            IHostingEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.hostingEnvironment = hostingEnvironment;

            serviceOfPost = new ServiceOfPost(context, mapper, hostingEnvironment);
            serviceOfImage = new ServiceOfImage(hostingEnvironment);
            serviceOfSection = new ServiceOfSection(context, mapper);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreatePost()
        {
            ViewData["Sections"] = serviceOfSection.Get();
            var postCreateEditViewModel =  serviceOfPost.CreateNotFinished(userManager.GetUserId(User));
            return View(postCreateEditViewModel);
        }
        [HttpPost]
        public IActionResult CreatePost(PostCreateEditViewModel postCreateEditViewModel)
        {
            serviceOfPost.CreateFinished(postCreateEditViewModel);
            return View();
        }


        public IActionResult CreateImage(int postId, IEnumerable<IFormFile> images)
        {
            var paths = images.Select(a => serviceOfImage.Create("Posts", postId.ToString(), a));
            return Content(paths.Count().ToString());
        }

        public IActionResult ListPostsViewModel()
        {
            var posts = serviceOfPost.Get<PostViewModel>(userManager.GetUserId(User));
            return View(posts);
        }
        public IActionResult ListNotFinishedPostsViewModel()
        {
            var posts = serviceOfPost.Get<PostViewModel>(userManager.GetUserId(User), false);
            return View(posts);
        }

        public IActionResult ListPostsMiniViewModel()
        {
            var posts = serviceOfPost.Get<PostMiniViewModel>(userManager.GetUserId(User));
            return View(posts);
        }
        public IActionResult ListNotFinishedPostsMiniViewModel()
        {
            var posts = serviceOfPost.Get<PostMiniViewModel>(userManager.GetUserId(User), false);
            return View(posts);
        }
    }
}