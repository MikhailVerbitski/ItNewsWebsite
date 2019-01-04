using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfPost
    {
        private readonly ApplicationDbContext context;
        private readonly DefaultRepository<PostEntity> defaultRepository;

        public RepositoryOfPost(ApplicationDbContext context)
        {
            this.context = context;

            defaultRepository = new DefaultRepository<PostEntity>(context);
        }
    }
}
