using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfApplicationUser : DefaultRepository<ApplicationUserEntity>
    {

        public RepositoryOfApplicationUser(ApplicationDbContext context) : base(context)
        { }

        public override ApplicationUserEntity Create(ApplicationUserEntity entity)
        {
            entity.Created = System.DateTime.Now;
            var ApplicationUser = base.Create(entity);

            RepositoryOfUserProfile repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            var UserProfile = repositoryOfUserProfile.Read(a => a.Id == ApplicationUser.UserProfileId);
            if(UserProfile == null)
            {
                UserProfile = new UserProfileEntity() { ApplicationUserId = ApplicationUser.Id };
                UserProfile = repositoryOfUserProfile.Create(UserProfile);
                ApplicationUser.UserProfileId = UserProfile.Id;
            }
            this.Update(ApplicationUser);

            return ApplicationUser;
        }

        public override void Delete(ApplicationUserEntity entity)
        {
            var repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            var userProfile = repositoryOfUserProfile.Read(a => a.Id == entity.UserProfileId);
            repositoryOfUserProfile.Delete(userProfile);
            base.Delete(entity);
        }
    }
}
