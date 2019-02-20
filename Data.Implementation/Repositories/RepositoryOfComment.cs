using Data.Contracts.Models.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfComment : DefaultRepository<CommentEntity>
    {
        public RepositoryOfComment(ApplicationDbContext context) : base(context)
        { }

        public override CommentEntity Create(CommentEntity entity)
        {
            entity.Created = System.DateTime.Now;
            return base.Create(entity);
        }

        public override void Delete(CommentEntity entity)
        {
            RepositoryOfCommentLike repositoryOfCommentLike = new RepositoryOfCommentLike(context);
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
