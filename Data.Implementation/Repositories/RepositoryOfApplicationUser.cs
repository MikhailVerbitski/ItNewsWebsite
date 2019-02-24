using System;
using System.Linq.Expressions;
using Data.Contracts.Models.Entities;
using Search.Implementation;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfApplicationUser : DefaultRepository<ApplicationUserEntity>
    {
        private readonly ServiceOfSearch serviceOfSearch;
        public RepositoryOfApplicationUser(ApplicationDbContext context, ServiceOfSearch serviceOfSearch) : base(context)
        {
            this.serviceOfSearch = serviceOfSearch;
        }

        public override ApplicationUserEntity Create(ApplicationUserEntity entity)
        {
            entity.Created = System.DateTime.Now;
            var ApplicationUser = base.Create(entity);
            serviceOfSearch.Create<ApplicationUserEntity>(ApplicationUser);
            RepositoryOfUserProfile repositoryOfUserProfile = new RepositoryOfUserProfile(context, serviceOfSearch);
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
        public override void Update(ApplicationUserEntity entity, params Expression<Func<ApplicationUserEntity, object>>[] properties)
        {
            base.Update(entity, properties);
            serviceOfSearch.Update<ApplicationUserEntity>(entity);
        }
        public override void Delete(ApplicationUserEntity entity)
        {
            var repositoryOfUserProfile = new RepositoryOfUserProfile(context, serviceOfSearch);
            var userProfile = repositoryOfUserProfile.Read(a => a.Id == entity.UserProfileId);
            serviceOfSearch.DeleteUser(entity);
            repositoryOfUserProfile.Delete(userProfile);
        }
    }
}
