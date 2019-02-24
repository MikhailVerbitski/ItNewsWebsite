using Data.Contracts.Models.Entities;
using Search.Implementation;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfComment : DefaultRepository<CommentEntity>
    {
        private readonly ServiceOfSearch serviceOfSearch;
        public RepositoryOfComment(ApplicationDbContext context, ServiceOfSearch serviceOfSearch) : base(context)
        {
            this.serviceOfSearch = serviceOfSearch;
        }
        public override CommentEntity Create(CommentEntity entity)
        {
            entity.Created = System.DateTime.Now;
            return base.Create(entity);
        }
        public override void Delete(CommentEntity entity)
        {
            RepositoryOfCommentLike repositoryOfCommentLike = new RepositoryOfCommentLike(context, serviceOfSearch);
            var commentLikes = entity.Likes;
            if(commentLikes == null)
            {
                commentLikes = repositoryOfCommentLike.ReadMany(new Expression<System.Func<CommentLikeEntity, bool>>[] { a => a.CommentId == entity.Id });
            }
            commentLikes = commentLikes.ToList();
            foreach (var item in commentLikes)
            {
                repositoryOfCommentLike.Delete(item, true);
            }
            base.Delete(entity);
        }
    }
}
