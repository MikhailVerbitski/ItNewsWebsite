using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Contracts;
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
            entity.Created = DateTime.Now;
            var ApplicationUser = base.Create(entity);
            serviceOfSearch.Create<ApplicationUserEntity>(ApplicationUser);
            IRepository<UserProfileEntity> repositoryOfUserProfile = new RepositoryOfUserProfile(context, serviceOfSearch);
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
        public override ApplicationUserEntity Update(ApplicationUserEntity entity, params Expression<Func<ApplicationUserEntity, object>>[] properties)
        {
            var applicationUserEntity = base.Update(entity, properties);
            serviceOfSearch.Update<ApplicationUserEntity>(entity);
            return applicationUserEntity;
        }
        public override async Task Delete(ApplicationUserEntity entity)
        {
            var repositoryOfUserProfile = new RepositoryOfUserProfile(context, serviceOfSearch);
            var userProfile = repositoryOfUserProfile.Read(a => a.Id == entity.UserProfileId);
            serviceOfSearch.DeleteUser(entity);
            await repositoryOfUserProfile.Delete(userProfile);
        }
    }
}
