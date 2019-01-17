using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Implementation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        UserManager<ApplicationUserEntity> userManager;
        //private readonly IMapper mapper;
        //private readonly IHostingEnvironment hostingEnvironment;

        //private readonly ServiceOfPost serviceOfPost;
        //private readonly ServiceOfImage serviceOfImage;
        //private readonly ServiceOfSection serviceOfSection;
        //private readonly RoleManager<IdentityRole> roleManager;
        //private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfComment serviceOfComment;

        public CommentController(
            UserManager<ApplicationUserEntity> userManager,
            //RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            //IHostingEnvironment hostingEnvironment,
            IMapper mapper
            )
        {
            this.userManager = userManager;
            //this.mapper = mapper;
            //this.hostingEnvironment = hostingEnvironment;
            //this.roleManager = roleManager;

            //serviceOfPost = new ServiceOfPost(context, mapper, hostingEnvironment);
            //serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            //serviceOfSection = new ServiceOfSection(context, mapper);
            //serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            serviceOfComment = new ServiceOfComment(context, mapper);
        }

        [HttpPost]
        [Route("CreateComment")]
        public void CreateComment(CommentCreateEditViewModel commentViewModel)
        {
            serviceOfComment.Create(userManager.GetUserId(User), commentViewModel);
        }
        [HttpGet("[action]")]
        [Route("LikeComment")]
        public void LikeComment(int commentId, int postId)
        {
            serviceOfComment.LikeComment(userManager.GetUserId(User), commentId);
        }
        [HttpGet("[action]")]
        [Route("DislikeComment")]
        public void DislikeComment(int commentId, int postId)
        {
            serviceOfComment.DislikeComment(userManager.GetUserId(User), commentId);
        }
    }
}
