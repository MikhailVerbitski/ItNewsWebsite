﻿using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels.Comment;
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
    [Route("api/Comment/[action]")]
    public class CommentController : Controller
    {
        public  readonly ServiceOfComment serviceOfComment;

        public CommentController(ServiceOfComment serviceOfComment)
        {
            this.serviceOfComment = serviceOfComment;
        }
        [AllowAnonymous]
        [HttpPost]
        public JsonResult Read([FromBody] CommentReadRequestParams requestParams)
        {
            var currentUserId = User.Claims.SingleOrDefault(a => a.Type == "UserId")?.Value;
            System.Collections.Generic.IEnumerable<BaseCommentViewModel> comments = null;
            if (requestParams.ApplicationUserId != null)
            {
                comments = serviceOfComment.GetMany(requestParams.type, requestParams.ApplicationUserId, requestParams.skip, requestParams.count, currentUserId);
            }
            else if (requestParams.PostId != null)
            {
                comments = serviceOfComment.GetMany(requestParams.type, requestParams.PostId.Value, requestParams.skip, requestParams.count, currentUserId);
            }
            return Json(comments);
        }
        [HttpPost]
        public async Task<JsonResult> Create([FromBody] CommentCreateEditViewModel commentViewModel)
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
