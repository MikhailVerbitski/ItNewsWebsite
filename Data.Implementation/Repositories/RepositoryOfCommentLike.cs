using System;
using System.Linq.Expressions;
using Data.Contracts.Models.Entities;

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

            var Comment = entity.Comment;
            if(Comment == null)
            {
                Comment = repositoryOfComment.Read(a => a.Id == entity.CommentId);
            }
            Comment.CountOfLikes++;
            repositoryOfComment.Update(Comment);

            var UserProfile = entity.UserProfile;
            if (UserProfile == null)
            {
                UserProfile = repositoryOfUserProfile.Read(a => a.Id == entity.UserProfileId, a => a.ApplicationUser);
            }
            var ApplicationUser = UserProfile.ApplicationUser;
            if (ApplicationUser == null)
            {
                ApplicationUser = repositoryOfApplicationUser.Read(a => a.Id == UserProfile.ApplicationUserId);
            }
            ApplicationUser.CountOfLikes++;
            repositoryOfApplicationUser.Update(ApplicationUser);

            return base.Create(entity);
        }
        public override void Delete(CommentLikeEntity entity)
        {
            RepositoryOfApplicationUser repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            RepositoryOfUserProfile repositoryOfUserProfile = new RepositoryOfUserProfile(context);

            var UserProfile = entity.UserProfile;
            if (UserProfile == null)
            {
                UserProfile = repositoryOfUserProfile.Read(a => a.Id == entity.UserProfileId, a => a.ApplicationUser);
            }
            var ApplicationUser = UserProfile.ApplicationUser;
            if (ApplicationUser == null)
            {
                ApplicationUser = repositoryOfApplicationUser.Read(a => a.Id == UserProfile.ApplicationUserId);
            }
            ApplicationUser.CountOfLikes--;
            repositoryOfApplicationUser.Update(ApplicationUser);

            base.Delete(entity);
        }
    }
}
