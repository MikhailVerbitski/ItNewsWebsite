﻿using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfCommentLike : DefaultRepository<CommentLikeEntity>
    {
        public RepositoryOfCommentLike(ApplicationDbContext context) : base(context)
        { }

        public override CommentLikeEntity Create(CommentLikeEntity entity)
        {
            RepositoryOfApplicationUser repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            RepositoryOfUserProfile repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            RepositoryOfComment repositoryOfComment = new RepositoryOfComment(context);

            var comment = entity.Comment;
            if(comment == null)
            {
                comment = repositoryOfComment.Read(a => a.Id == entity.CommentId);
            }
            comment.CountOfLikes++;
            repositoryOfComment.Update(comment);
            
            var ApplicationUserOfComment = repositoryOfApplicationUser.Read(a => a.UserProfileId == comment.UserProfileId);
            ApplicationUserOfComment.CountOfLikes++;
            repositoryOfApplicationUser.Update(ApplicationUserOfComment);

            return base.Create(entity);
        }
        public override void Delete(CommentLikeEntity entity)
        {
            RepositoryOfApplicationUser repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            RepositoryOfUserProfile repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            RepositoryOfComment repositoryOfComment = new RepositoryOfComment(context);

            var comment = entity.Comment;
            if (comment == null)
            {
                comment = repositoryOfComment.Read(a => a.Id == entity.CommentId);
            }
            comment.CountOfLikes--;
            repositoryOfComment.Update(comment);
            
            var ApplicationUserOfComment = repositoryOfApplicationUser.Read(a => a.UserProfileId == comment.UserProfileId);
            ApplicationUserOfComment.CountOfLikes--;
            repositoryOfApplicationUser.Update(ApplicationUserOfComment);

            base.Delete(entity);
        }
    }
}
