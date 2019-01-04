using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfComment
    {
        private readonly ApplicationDbContext context;
        private readonly DefaultRepository<CommentEntity> defaultRepository;

        public RepositoryOfComment(ApplicationDbContext context)
        {
            this.context = context;

            defaultRepository = new DefaultRepository<CommentEntity>(context);
        }
    }
}
