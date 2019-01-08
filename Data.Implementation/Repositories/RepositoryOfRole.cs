using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfRole : DefaultRepository<RoleEntity>
    {
        public RepositoryOfRole(ApplicationDbContext context) : base(context)
        { }
    }
}
