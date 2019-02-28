using Data.Contracts;
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
            IRepository<ApplicationUserEntity> repositoryOfApplicationUser = new RepositoryOfApplicationUser(context, serviceOfSearch);
            IRepository<CommentEntity> repositoryOfComment = new RepositoryOfComment(context, serviceOfSearch);
            
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
            IRepository<ApplicationUserEntity> repositoryOfApplicationUser = new RepositoryOfApplicationUser(context, serviceOfSearch);
            IRepository<CommentEntity> repositoryOfComment = new RepositoryOfComment(context, serviceOfSearch);
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
