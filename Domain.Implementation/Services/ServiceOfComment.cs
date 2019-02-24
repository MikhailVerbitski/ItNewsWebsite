using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.User;
using Search.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly Tuple<string, Func<CommentEntity, ApplicationUserEntity, BaseCommentViewModel>>[] Config;

        public ServiceOfComment(ApplicationDbContext context, IMapper mapper, ServiceOfUser serviceOfUser, ServiceOfSearch serviceOfSearch)
        {
            this.mapper = mapper;

            repositoryOfPost = new RepositoryOfPost(context, serviceOfSearch);
            repositoryOfComment = new RepositoryOfComment(context, serviceOfSearch);
            repositoryOfCommentLike = new RepositoryOfCommentLike(context, serviceOfSearch);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context, serviceOfSearch);
            repositoryOfUserProfile = new RepositoryOfUserProfile(context, serviceOfSearch);

            this.serviceOfUser = serviceOfUser;

            Config = new[]
            {
                new Tuple<string, Func<CommentEntity, ApplicationUserEntity, BaseCommentViewModel>>(nameof(CommentViewModel), GetCommentViewModel),
                new Tuple<string, Func<CommentEntity, ApplicationUserEntity, BaseCommentViewModel>>(nameof(CommentMiniViewModel), GetCommentMiniViewModel)
            };
        }

        public async Task<CommentViewModel> Create(string applicationUserIdCurrent, CommentCreateEditViewModel commentCreateEditViewModel)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if (await serviceOfUser.GetUserPriority(applicationUser) >= 1)
            {
                var commentEntity = mapper.Map<CommentCreateEditViewModel, CommentEntity>(commentCreateEditViewModel);
                commentEntity.UserProfileId = applicationUser.UserProfileId;
                commentEntity = repositoryOfComment.Create(commentEntity);
                var commentViewModel = GetViewModelWithProperty(nameof(CommentViewModel), commentEntity, applicationUserIdCurrent);
                return commentViewModel as CommentViewModel;
            }
            return null;
        }

        public IEnumerable<BaseCommentViewModel> GetMany(string type, string applicationUserId, int? skip, int? take, string applicationUserIdCurrent)
        {
            var user = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            return GetMany(type, user, skip, take, applicationUserId);
        }
        public IEnumerable<BaseCommentViewModel> GetMany(string type, ApplicationUserEntity applicationUser, int? skip, int? take, string applicationUserIdCurrent)
        {
            var commentEntities = repositoryOfComment.ReadMany(new System.Linq.Expressions.Expression<Func<CommentEntity, bool>>[] { a => a.UserProfileId == applicationUser.UserProfileId });
            commentEntities = commentEntities.OrderBy(a => a.Created).Reverse();
            return GetMany(type, commentEntities, skip, take, applicationUserIdCurrent);
        }
        public IEnumerable<BaseCommentViewModel> GetMany(string type, int postId, int? skip, int? take, string applicationUserIdCurrent)
        {
            var commentEntities = repositoryOfComment.ReadMany(new System.Linq.Expressions.Expression<Func<CommentEntity, bool>>[] { a => a.PostId == postId });
            commentEntities = commentEntities.OrderBy(a => a.Created).Reverse();
            return GetMany(type, commentEntities, skip, take, applicationUserIdCurrent);
        }
        public IEnumerable<BaseCommentViewModel> GetMany(string type, int postId, int? skip, int? take, ApplicationUserEntity applicationUserCurrent)
        {
            var commentEntities = repositoryOfComment.ReadMany(new System.Linq.Expressions.Expression<Func<CommentEntity, bool>>[] { a => a.PostId == postId });
            commentEntities = commentEntities.OrderBy(a => a.Created).Reverse();
            return GetMany(type, commentEntities, skip, take, applicationUserCurrent);
        }
        public IEnumerable<BaseCommentViewModel> GetMany(string type, IEnumerable<CommentEntity> commentEntities, int? skip, int? take, ApplicationUserEntity applicationUserCurrent)
        {
            if (skip != null)
            {
                commentEntities = commentEntities.Skip(skip.Value);
            }
            if (take != null)
            {
                commentEntities = commentEntities.Take(take.Value);
            }
            return commentEntities.Select(a => GetViewModelWithProperty(type, a, applicationUserCurrent)).ToList();
        }
        public IEnumerable<BaseCommentViewModel> GetMany(string type, IEnumerable<CommentEntity> commentEntities, int? skip, int? take, string applicationUserIdCurrent)
        {
            if (skip != null)
            {
                commentEntities = commentEntities.Skip(skip.Value);
            }
            if (take != null)
            {
                commentEntities = commentEntities.Take(take.Value);
            }
            return commentEntities.Select(a => GetViewModelWithProperty(type, a, applicationUserIdCurrent)).ToList();
        }

        public BaseCommentViewModel Get(string type, int commentId, string applicationUserIdCurrent) 
        {
            var commentEntity = repositoryOfComment.Read(a => a.Id == commentId);
            return Get(type, commentEntity, applicationUserIdCurrent);
        }
        public BaseCommentViewModel Get(string type, int commentId, ApplicationUserEntity applicationUserCurrent)
        {
            var commentEntity = repositoryOfComment.Read(a => a.Id == commentId);
            return Get(type, commentEntity, applicationUserCurrent);
        }
        public BaseCommentViewModel Get(string type, CommentEntity commentEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var commentViewModel = GetViewModelWithProperty(type, commentEntity, applicationUserCurrent);
            return commentViewModel;
        }
        public BaseCommentViewModel Get(string type, CommentEntity commentEntity, string applicationUserIdCurrent)
        {
            var commentViewModel = GetViewModelWithProperty(type, commentEntity, applicationUserIdCurrent);
            return commentViewModel;
        }

        private CommentViewModel GetCommentViewModel(CommentEntity commentEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var commentViewModel = mapper.Map<CommentEntity, CommentViewModel>(commentEntity);
            var applicationUserForComment = repositoryOfUserProfile.Read(a => a.Id == commentEntity.UserProfileId, a => a.ApplicationUser).ApplicationUser;
            UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserForComment);
            commentViewModel.BelongsToUser = (applicationUserCurrent == null) ? false : applicationUserCurrent.UserProfileId == commentEntity.UserProfileId;
            commentViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserForComment);
            commentViewModel.IsUserLiked = (applicationUserCurrent == null) ? false : IsCommentLike(commentEntity, applicationUserCurrent.UserProfileId.Value);
            return commentViewModel;
        }
        private CommentMiniViewModel GetCommentMiniViewModel(CommentEntity commentEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var commentViewModel = mapper.Map<CommentEntity, CommentMiniViewModel>(commentEntity);
            var applicationUserForComment = repositoryOfUserProfile.Read(a => a.Id == commentEntity.UserProfileId, a => a.ApplicationUser).ApplicationUser;
            UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserForComment);
            commentViewModel.PostHeader = commentEntity.Post == null ? repositoryOfPost.Read(a => a.Id == commentEntity.PostId).Header : commentEntity.Post.Header;
            commentViewModel.BelongsToUser = (applicationUserCurrent == null) ? false : applicationUserCurrent.UserProfileId == commentEntity.UserProfileId;
            commentViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserForComment);
            return commentViewModel;
        }
        private CommentCreateEditViewModel GetCommentCreateEditViewModel(CommentEntity commentEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var commentViewModel = mapper.Map<CommentEntity, CommentCreateEditViewModel>(commentEntity);
            return commentViewModel;
        }

        public BaseCommentViewModel GetViewModelWithProperty(string type, CommentEntity commentEntity, string applicationUserIdCurrent) 
        {
            var applicationUser = (applicationUserIdCurrent != null) ? repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent, a => a.UserProfile) : null;
            return GetViewModelWithProperty(type, commentEntity, applicationUser);
        }
        public BaseCommentViewModel GetViewModelWithProperty(string type, CommentEntity commentEntity, ApplicationUserEntity applicationUserEntity)
        {
            return Config.Single(a => a.Item1 == type).Item2(commentEntity, applicationUserEntity);
        }

        public async Task<CommentViewModel> Update(string applicationUserIdCurrent, CommentCreateEditViewModel commentCreateEditViewModel)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if (await serviceOfUser.GetUserPriority(applicationUser) >= 1)
            {
                var commentEntity = mapper.Map<CommentCreateEditViewModel, CommentEntity>(commentCreateEditViewModel);
                repositoryOfComment.Update(commentEntity);
                commentEntity = repositoryOfComment.Read(a => a.Id == commentCreateEditViewModel.CommentId);
                var commentViewModel = mapper.Map<CommentEntity, CommentViewModel>(commentEntity);
                return commentViewModel;
            }
            return null;
        }
        public async Task LikeComment(string applicationUserIdCurrent, int commentId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if(await serviceOfUser.GetUserPriority(applicationUser) >= 1)
            {
                var commentLike = new CommentLikeEntity()
                {
                    CommentId = commentId,
                    UserProfileId = applicationUser.UserProfileId
                };
                var commentLikeEntity = repositoryOfCommentLike.Create(commentLike);
            }
        }
        public async Task DislikeComment(string applicationUserIdCurrent, int commentId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if (await serviceOfUser.GetUserPriority(applicationUser) >= 1)
            {
                var commentLike = repositoryOfCommentLike.Read(a => a.CommentId == commentId && a.UserProfileId == applicationUser.UserProfileId);
                repositoryOfCommentLike.Delete(commentLike);
            }
        }
        public async Task Delete(string applicationUserIdCurrent, int commentId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if (await serviceOfUser.GetUserPriority(applicationUser) >= 1)
            {
                var comment = repositoryOfComment.Read(a => a.Id == commentId);
                repositoryOfComment.Delete(comment);
            }
        }
        public bool IsCommentLike(CommentEntity commentEntity, int userProfileId)
        {
            var commentLikeEntity = repositoryOfCommentLike.Read(a => a.CommentId == commentEntity.Id && a.UserProfileId == userProfileId);
            return commentLikeEntity != null;
        }
    }
}