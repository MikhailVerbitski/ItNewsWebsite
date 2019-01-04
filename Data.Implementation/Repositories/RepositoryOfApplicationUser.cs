using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfApplicationUser
    {
        private readonly ApplicationDbContext context;
        private readonly DefaultRepository<ApplicationUserEntity> defaultRepository;

        public RepositoryOfApplicationUser(ApplicationDbContext context)
        {
            this.context = context;

            defaultRepository = new DefaultRepository<ApplicationUserEntity>(context);
        }
    }
}
