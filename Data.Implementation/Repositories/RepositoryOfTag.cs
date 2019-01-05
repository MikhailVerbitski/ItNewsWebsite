using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfTag : DefaultRepository<TagEntity>
    {
        public RepositoryOfTag(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

    }
}
