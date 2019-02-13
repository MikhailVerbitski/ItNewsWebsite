using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.User;
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

        private readonly Tuple<Type, Func<CommentEntity, ApplicationUserEntity, BaseCommentViewModel>>[] Config;

        public ServiceOfComment(
            ApplicationDbContext context,
            IMapper mapper,
            ServiceOfUser serviceOfUser
            )
        {
            this.mapper = mapper;

            repositoryOfPost = new RepositoryOfPost(context);
            repositoryOfComment = new RepositoryOfComment(context);
            repositoryOfCommentLike = new RepositoryOfCommentLike(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfUserProfile = new RepositoryOfUserProfile(context);

            this.serviceOfUser = serviceOfUser;

            Config = new[]
            {
                new Tuple<Type, Func<CommentEntity, ApplicationUserEntity, BaseCommentViewModel>>(typeof(CommentViewModel), GetCommentViewModel),
                new Tuple<Type, Func<CommentEntity, ApplicationUserEntity, BaseCommentViewModel>>(typeof(CommentMiniViewModel), GetCommentMiniViewModel)
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
                var commentViewModel = GetViewModelWithProperty<CommentViewModel>(commentEntity, applicationUserIdCurrent);
                return commentViewModel as CommentViewModel;
            }
            return null;
        }

        public IEnumerable<TCommentViewModel> GetMany<TCommentViewModel>(int postId, string applicationUserIdCurrent) 
            where TCommentViewModel : BaseCommentViewModel
        {
            var post = repositoryOfPost.Read(a => a.Id == postId, a => a.Comments);
            return GetMany<TCommentViewModel>(post, applicationUserIdCurrent);
        }
        public IEnumerable<TCommentViewModel> GetMany<TCommentViewModel>(int postId, ApplicationUserEntity applicationUserCurrent)
            where TCommentViewModel : BaseCommentViewModel
        {
            var post = repositoryOfPost.Read(a => a.Id == postId, a => a.Comments);
            return GetMany<TCommentViewModel>(post, applicationUserCurrent);
        }
        public IEnumerable<TCommentViewModel> GetMany<TCommentViewModel>(PostEntity post, string applicationUserIdCurrent)
            where TCommentViewModel : BaseCommentViewModel
        {
            if (post.Comments == null)
            {
                post = repositoryOfPost.Read(a => a.Id == post.Id, a => a.Comments);
            }
            return GetMany<TCommentViewModel>(post, applicationUserIdCurrent);
        }
        public IEnumerable<TCommentViewModel> GetMany<TCommentViewModel>(PostEntity post, ApplicationUserEntity applicationUserCurrent)
            where TCommentViewModel : BaseCommentViewModel
        {
            if(post.Comments == null)
            {
                post = repositoryOfPost.Read(a => a.Id == post.Id, a => a.Comments);
            }
            return post.Comments.Select(a => Get<TCommentViewModel>(a, applicationUserCurrent)).AsParallel().ToList();
        }
        public TCommentViewModel Get<TCommentViewModel>(int commentId, string applicationUserIdCurrent) 
            where TCommentViewModel : BaseCommentViewModel
        {
            var commentEntity = repositoryOfComment.Read(a => a.Id == commentId);
            return Get<TCommentViewModel>(commentEntity, applicationUserIdCurrent);
        }
        public TCommentViewModel Get<TCommentViewModel>(int commentId, ApplicationUserEntity applicationUserCurrent)
            where TCommentViewModel : BaseCommentViewModel
        {
            var commentEntity = repositoryOfComment.Read(a => a.Id == commentId);
            return Get<TCommentViewModel>(commentEntity, applicationUserCurrent);
        }
        public TCommentViewModel Get<TCommentViewModel>(CommentEntity commentEntity, ApplicationUserEntity applicationUserCurrent)
            where TCommentViewModel : BaseCommentViewModel
        {
            var commentViewModel = GetViewModelWithProperty<TCommentViewModel>(commentEntity, applicationUserCurrent);
            return commentViewModel as TCommentViewModel;
        }
        public TCommentViewModel Get<TCommentViewModel>(CommentEntity commentEntity, string applicationUserIdCurrent)
            where TCommentViewModel : BaseCommentViewModel
        {
            var commentViewModel = GetViewModelWithProperty<TCommentViewModel>(commentEntity, applicationUserIdCurrent);
            return commentViewModel as TCommentViewModel;
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

        public TCommentViewModel GetViewModelWithProperty<TCommentViewModel>(CommentEntity commentEntity, string applicationUserIdCurrent) 
            where TCommentViewModel: BaseCommentViewModel
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent, a => a.UserProfile);
            return GetViewModelWithProperty<TCommentViewModel>(commentEntity, applicationUser);
        }
        public TCommentViewModel GetViewModelWithProperty<TCommentViewModel>(CommentEntity commentEntity, ApplicationUserEntity applicationUserEntity)
            where TCommentViewModel : BaseCommentViewModel
        {
            return Config.Single(a => a.Item1 == typeof(TCommentViewModel)).Item2(commentEntity, applicationUserEntity) as TCommentViewModel;
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