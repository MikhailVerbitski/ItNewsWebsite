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
        public void Delete(CommentLikeEntity entity, bool forCommentDelete = false)
        {
            RepositoryOfApplicationUser repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            RepositoryOfComment repositoryOfComment = new RepositoryOfComment(context);

            var comment = entity.Comment;
            if (comment == null)
            {
                comment = repositoryOfComment.Read(a => a.Id == entity.CommentId);
            }

            if (!forCommentDelete)
            {
                comment.CountOfLikes--;
                repositoryOfComment.Update(comment);
            }
            
            var ApplicationUserOfComment = repositoryOfApplicationUser.Read(a => a.UserProfileId == comment.UserProfileId);
            ApplicationUserOfComment.CountOfLikes--;
            repositoryOfApplicationUser.Update(ApplicationUserOfComment);

            base.Delete(entity);
        }
    }
}
