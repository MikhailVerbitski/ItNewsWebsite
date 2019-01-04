using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    class RepositoryOfUserProfile
    {
        private readonly ApplicationDbContext context;
        private readonly DefaultRepository<UserProfileEntity> defaultRepository;

        public RepositoryOfUserProfile(ApplicationDbContext context)
        {
            this.context = context;

            defaultRepository = new DefaultRepository<UserProfileEntity>(context);
        }
    }
}
