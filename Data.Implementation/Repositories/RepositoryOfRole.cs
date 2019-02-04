using Microsoft.AspNetCore.Identity;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfRole : DefaultRepository<IdentityRole<string>>
    {
        public RepositoryOfRole(ApplicationDbContext context) : base(context)
        {

        }
    }
}
