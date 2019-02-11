﻿using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models.ViewModels.Comment;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "user, admin")]
    [ApiController]
    [Route("api/Comment/[action]")]
    public class CommentController : Controller
    {
        public  readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfPost serviceOfPost;
        private readonly ServiceOfAccount serviceOfAccount;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfTag serviceOfTag;
        private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfRole serviceOfRole;

        public CommentController(
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<RoleEntity> roleManager,
            ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper
            )
        {
            serviceOfRole = new ServiceOfRole(context, userManager, roleManager, mapper);
            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfAccount = new ServiceOfAccount(context, userManager, roleManager, hostingEnvironment, mapper);
            serviceOfComment = new ServiceOfComment(context, mapper, serviceOfUser);
            serviceOfPost = new ServiceOfPost(context, mapper, serviceOfImage, serviceOfAccount, serviceOfComment, serviceOfUser, serviceOfTag, serviceOfRole);
            serviceOfUser = new ServiceOfUser(context, userManager, mapper, serviceOfImage, serviceOfAccount, serviceOfComment, serviceOfPost, serviceOfRole);
            serviceOfTag = new ServiceOfTag(context, mapper);

            serviceOfComment.serviceOfUser = serviceOfUser;
            serviceOfPost.serviceOfUser = serviceOfUser;
            serviceOfPost.serviceOfTag = serviceOfTag;
            serviceOfUser.serviceOfPost = serviceOfPost;
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
