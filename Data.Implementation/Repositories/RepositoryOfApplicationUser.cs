using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfApplicationUser : DefaultRepository<ApplicationUserEntity>
    {

        public RepositoryOfApplicationUser(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public override ApplicationUserEntity Create(ApplicationUserEntity entity)
        {
            var ApplicationUser = base.Create(entity);

            RepositoryOfUserProfile repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            var UserProfile = repositoryOfUserProfile.Read(a => a.Id == ApplicationUser.UserProfileId);
            if(UserProfile.ApplicationUserId == null)
            {
                UserProfile.ApplicationUserId = ApplicationUser.Id;
            }
            repositoryOfUserProfile.Update(UserProfile);

            return ApplicationUser;
        }
    }
}
