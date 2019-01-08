using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfUserProfile : DefaultRepository<UserProfileEntity>
    {
        public RepositoryOfUserProfile(ApplicationDbContext context) : base(context)
        { }
    }
}
