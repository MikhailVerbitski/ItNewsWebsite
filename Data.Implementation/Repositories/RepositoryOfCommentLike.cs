using Data.Contracts.Models.Entities;
using Search.Implementation;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfCommentLike : DefaultRepository<CommentLikeEntity>
    {
        private readonly ServiceOfSearch serviceOfSearch;
        public RepositoryOfCommentLike(ApplicationDbContext context, ServiceOfSearch serviceOfSearch) : base(context)
        {
            this.serviceOfSearch = serviceOfSearch;
        }

        public override CommentLikeEntity Create(CommentLikeEntity entity)
        {
            RepositoryOfApplicationUser repositoryOfApplicationUser = new RepositoryOfApplicationUser(context, serviceOfSearch);
            RepositoryOfComment repositoryOfComment = new RepositoryOfComment(context, serviceOfSearch);
            
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
            RepositoryOfApplicationUser repositoryOfApplicationUser = new RepositoryOfApplicationUser(context, serviceOfSearch);
            RepositoryOfComment repositoryOfComment = new RepositoryOfComment(context, serviceOfSearch);

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
