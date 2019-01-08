using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfImage : DefaultRepository<ImageEntity>
    {
        public RepositoryOfImage(ApplicationDbContext context) : base(context)
        { }
    }
}
