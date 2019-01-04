using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfTag
    {
        private readonly ApplicationDbContext context;
        private readonly DefaultRepository<TagEntity> defaultRepository;

        public RepositoryOfTag(ApplicationDbContext context)
        {
            this.context = context;

            defaultRepository = new DefaultRepository<TagEntity>(context);
        }
    }
}
