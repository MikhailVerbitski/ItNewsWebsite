using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
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

        private readonly Tuple<Type, Func<CommentEntity, string, BaseCommentViewModel>>[] Config;

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

            Config = new[]
            {
                new Tuple<Type, Func<CommentEntity, string, BaseCommentViewModel>>(typeof(CommentViewModel), GetCommentViewModel),
                new Tuple<Type, Func<CommentEntity, string, BaseCommentViewModel>>(typeof(CommentMiniViewModel), GetCommentMiniViewModel),
                new Tuple<Type, Func<CommentEntity, string, BaseCommentViewModel>>(typeof(CommentCreateEditViewModel), GetCommentCreateEditViewModel)
            };
        }

        public CommentViewModel Create(string applicationUserId, CommentCreateEditViewModel commentCreateEditViewModel)
        {
            var commentEntity = mapper.Map<CommentCreateEditViewModel, CommentEntity>(commentCreateEditViewModel);
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            commentEntity.UserProfileId = applicationUser.UserProfileId;
            commentEntity = repositoryOfComment.Create(commentEntity);
            var commentViewModel = GetViewModelWithProperty<CommentViewModel>(commentEntity, applicationUserId);
            return commentViewModel as CommentViewModel;
        }

        public IEnumerable<TCommentViewModel> Get<TCommentViewModel>(int postId, string applicationUserIdCurrent) 
            where TCommentViewModel : BaseCommentViewModel
        {
            var post = repositoryOfPost.Read(a => a.Id == postId, a => a.Comments);
            var commentEntities = post.Comments;
            return commentEntities
                .Select(a => GetViewModelWithProperty<TCommentViewModel>(a, applicationUserIdCurrent) as TCommentViewModel)
                .AsParallel()
                .ToList();
        }

        public TCommentViewModel Get<TCommentViewModel>(string applicationUserIdCurrent, int commentId) 
            where TCommentViewModel : BaseCommentViewModel
        {
            var commentEntity = repositoryOfComment.Read(a => a.Id == commentId);
            var commentViewModel = GetViewModelWithProperty<TCommentViewModel>(commentEntity, applicationUserIdCurrent);
            return commentViewModel as TCommentViewModel;
        }

        private CommentViewModel GetCommentViewModel(CommentEntity commentEntity, string applicationUserIdCurrent)
        {
            var commentViewModel = mapper.Map<CommentEntity, CommentViewModel>(commentEntity);
            ApplicationUserEntity applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            var applicationUserForComment = repositoryOfUserProfile.Read(a => a.Id == commentEntity.UserProfileId, a => a.ApplicationUser).ApplicationUser;
            UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserForComment);
            commentViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserForComment);
            commentViewModel.IsUserLiked = IsCommentLike(commentEntity, applicationUserCurrent.UserProfileId.Value);
            return commentViewModel;
        }
        private CommentMiniViewModel GetCommentMiniViewModel(CommentEntity commentEntity, string applicationUserIdCurrent)
        {
            var commentViewModel = mapper.Map<CommentEntity, CommentMiniViewModel>(commentEntity);
            var applicationUserForComment = repositoryOfUserProfile.Read(a => a.Id == commentEntity.UserProfileId, a => a.ApplicationUser).ApplicationUser;
            UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserForComment);
            commentViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserForComment);
            return commentViewModel;
        }
        private CommentCreateEditViewModel GetCommentCreateEditViewModel(CommentEntity commentEntity, string applicationUserIdCurrent)
        {
            var commentViewModel = mapper.Map<CommentEntity, CommentCreateEditViewModel>(commentEntity);
            var applicationUserForComment = repositoryOfUserProfile.Read(a => a.Id == commentEntity.UserProfileId, a => a.ApplicationUser).ApplicationUser;
            UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserForComment);
            commentViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserForComment);
            return commentViewModel;
        }

        private BaseCommentViewModel GetViewModelWithProperty<TCommentViewModel>(CommentEntity commentEntity, string applicationUserIdCurrent) =>
            Config.Single(a => a.Item1 == typeof(TCommentViewModel)).Item2(commentEntity, applicationUserIdCurrent);

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

        public bool IsCommentLike(CommentEntity commentEntity, int userProfileId)
        {
            var commentLikeEntity = repositoryOfCommentLike.Read(a => a.CommentId == commentEntity.Id && a.UserProfileId == userProfileId);
            return commentLikeEntity != null;
        }
    }
}