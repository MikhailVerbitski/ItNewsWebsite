using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
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

        public ServiceOfComment(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;

            repositoryOfPost = new RepositoryOfPost(context);
            repositoryOfComment = new RepositoryOfComment(context);
            repositoryOfCommentLike = new RepositoryOfCommentLike(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
        }

        public CommentViewModel Create(string applicationUserId, CommentCreateEditViewModel commentCreateEditViewModel)
        {
            var commentEntity = mapper.Map<CommentCreateEditViewModel, CommentEntity>(commentCreateEditViewModel);
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            commentEntity.UserProfileId = applicationUser.UserProfileId;
            commentEntity = repositoryOfComment.Create(commentEntity);
            var commentViewModel = mapper.Map<CommentEntity, CommentViewModel>(commentEntity);
            return commentViewModel;
        }
        
        public IEnumerable<TCommentViewModel> Get<TCommentViewModel>(int PostId) where TCommentViewModel : class
        {
            var post = repositoryOfPost.Read(a => a.Id == PostId, a => a.Comments);
            var commentEntities = post.Comments.ToList();
            var commentViewModel = mapper.Map<IEnumerable<CommentEntity>, IEnumerable<TCommentViewModel>>(commentEntities);
            return commentViewModel.ToList();
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
        

        private void Delete<TCommentViewModel>(TCommentViewModel commentViewModel)
        {
            var commentEntity = mapper.Map<TCommentViewModel, CommentEntity>(commentViewModel);
            repositoryOfComment.Delete(commentEntity);
        }
    }
}