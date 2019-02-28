using Microsoft.AspNetCore.Identity;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfUserClaim : DefaultRepository<IdentityUserClaim<string>>
    {
        public RepositoryOfUserClaim(ApplicationDbContext context):base(context)
        { }
    }
}
