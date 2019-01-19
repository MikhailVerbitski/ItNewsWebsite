using Data.Contracts.Models.Entities;

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
    }
}
