using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Implementation.Services
{
    public class ServiceOfComment
    {
        private readonly IMapper mapper;

        private readonly RepositoryOfPost repositoryOfPost;
        private readonly RepositoryOfComment repositoryOfComment;
        private readonly RepositoryOfCommentLike repositoryOfCommentLike;
        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfUserProfile repositoryOfUserProfile;

        private readonly ServiceOfUser serviceOfUser;

        public ServiceOfComment(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUserEntity> userManager,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper
            )
        {
            this.mapper = mapper;

            repositoryOfPost = new RepositoryOfPost(context);
            repositoryOfComment = new RepositoryOfComment(context);
            repositoryOfCommentLike = new RepositoryOfCommentLike(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfUserProfile = new RepositoryOfUserProfile(context);

            serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
        }

        public CommentViewModel Create(string applicationUserId, CommentCreateEditViewModel commentCreateEditViewModel)
        {
            var commentEntity = mapper.Map<CommentCreateEditViewModel, CommentEntity>(commentCreateEditViewModel);
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            commentEntity.UserProfileId = applicationUser.UserProfileId;
            commentEntity = repositoryOfComment.Create(commentEntity);
            var commentViewModel = GetViewModelWithProperty<CommentViewModel>(commentEntity, applicationUserId);
            return commentViewModel;
        }

        public IEnumerable<TCommentViewModel> Get<TCommentViewModel>(int postId, string applicationUserIdCurrent) 
            where TCommentViewModel : class
        {
            var post = repositoryOfPost.Read(a => a.Id == postId, a => a.Comments);
            var commentEntities = post.Comments;
            var commentViewModel = commentEntities.Select(a => GetViewModelWithProperty<TCommentViewModel>(a, applicationUserIdCurrent));
            return commentViewModel.ToList();
        }

        public TCommentViewModel Get<TCommentViewModel>(string applicationUserIdCurrent, int commentId) 
            where TCommentViewModel : class
        {
            var commentEntity = repositoryOfComment.Read(a => a.Id == commentId);
            var commentViewModel = GetViewModelWithProperty<TCommentViewModel>(commentEntity, applicationUserIdCurrent);
            return commentViewModel;
        }

        private TCommentViewModel GetViewModelWithProperty<TCommentViewModel>(CommentEntity commentEntity, string applicationUserIdCurrent)
            where TCommentViewModel : class
        {
            var commentViewModel = mapper.Map<CommentEntity, TCommentViewModel>(commentEntity);
            ApplicationUserEntity applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);

            var propertyAuthorUserMiniViewModel = typeof(TCommentViewModel).GetProperty("AuthorUserMiniViewModel");
            var propertyIsUserLiked = typeof(TCommentViewModel).GetProperty("IsUserLiked");

            if(propertyAuthorUserMiniViewModel != null)
            {
                var applicationUserForComment = repositoryOfUserProfile.Read(a => a.Id == commentEntity.UserProfileId, a => a.ApplicationUser).ApplicationUser;
                UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserForComment);
                var role = serviceOfUser.GetUserRole(applicationUserForComment).Result;
                userMiniViewModel.Role = role.Item1;
                userMiniViewModel.RoleColor = role.Item2;
                propertyAuthorUserMiniViewModel.SetValue(commentViewModel, userMiniViewModel);
            }
            if(propertyIsUserLiked != null)
            {
                var commentLikeEntity = repositoryOfCommentLike.Read(a => a.CommentId == commentEntity.Id && a.UserProfileId == applicationUserCurrent.UserProfileId);
                var isUserLiked = commentLikeEntity != null;
                propertyIsUserLiked.SetValue(commentViewModel, isUserLiked);
            }
            return commentViewModel;
        }

        public CommentViewModel Update(CommentCreateEditViewModel commentCreateEditViewModel)
        {
            var commentEntity = mapper.Map<CommentCreateEditViewModel, CommentEntity>(commentCreateEditViewModel);
            repositoryOfComment.Update(commentEntity);
            commentEntity = repositoryOfComment.Read(a => a.Id == commentCreateEditViewModel.CommentId);
            var commentViewModel = mapper.Map<CommentEntity, CommentViewModel>(commentEntity);
            return commentViewModel;
        }

        public void LikeComment(string applicationUserId, int commentId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var commentLike = new CommentLikeEntity()
            {
                CommentId = commentId,
                UserProfileId = applicationUser.UserProfileId
            };
            var commentLikeEntity = repositoryOfCommentLike.Create(commentLike);
        }

        public void DislikeComment(string applicationUserId, int commentId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var commentLike = repositoryOfCommentLike.Read(a => a.CommentId == commentId && a.UserProfileId == applicationUser.UserProfileId);
            repositoryOfCommentLike.Delete(commentLike);
        }

        public void Delete<TCommentViewModel>(TCommentViewModel commentViewModel)
        {
            var commentEntity = mapper.Map<TCommentViewModel, CommentEntity>(commentViewModel);
            repositoryOfComment.Delete(commentEntity);
        }
    }
}