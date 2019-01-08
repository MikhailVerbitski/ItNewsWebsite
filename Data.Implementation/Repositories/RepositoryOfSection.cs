using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfSection : DefaultRepository<SectionEntity>
    {
        public RepositoryOfSection(ApplicationDbContext context) : base(context)
        { }
    }
}
