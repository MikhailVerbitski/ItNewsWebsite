using Microsoft.AspNetCore.Identity;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfIdentityUserRole : DefaultRepository<IdentityUserRole<string>>
    {
        public RepositoryOfIdentityUserRole(ApplicationDbContext context) : base(context)
        { }
    }
}
