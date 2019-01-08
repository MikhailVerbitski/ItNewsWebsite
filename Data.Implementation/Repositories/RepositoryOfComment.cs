using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfComment : DefaultRepository<CommentEntity>
    {
        public RepositoryOfComment(ApplicationDbContext context) : base(context)
        { }
    }
}
