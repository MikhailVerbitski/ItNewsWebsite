using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    class RepositoryOfSection
    {
        private readonly ApplicationDbContext context;
        private readonly DefaultRepository<SectionEntity> defaultRepository;

        public RepositoryOfSection(ApplicationDbContext context)
        {
            this.context = context;

            defaultRepository = new DefaultRepository<SectionEntity>(context);
        }
    }
}
