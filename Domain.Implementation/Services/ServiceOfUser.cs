using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Account;

namespace Domain.Implementation.Services
{
    public class ServiceOfUser
    {
        private readonly IMapper mapper;

        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfUserProfile repositoryOfUserProfile;

        public ServiceOfUser(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;

            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfUserProfile = new RepositoryOfUserProfile(context);
        }

        public ApplicationUserEntity GetApplicationUser(RegisterViewModel registerViewModel)
        {
            var applicationUser = mapper.Map<RegisterViewModel, ApplicationUserEntity>(registerViewModel);

            var applicationUsercheck = repositoryOfApplicationUser.Read(a => a.UserName == registerViewModel.Login);
            if (applicationUsercheck == null)
            {
                applicationUser = repositoryOfApplicationUser.Create(applicationUser);
            }
            else
            {
                applicationUser = applicationUsercheck;
            }
            if(applicationUser.UserProfileId == null)
            {
                var userProfile = new UserProfileEntity();
                userProfile.ApplicationUserId = applicationUser.Id;
                userProfile = repositoryOfUserProfile.Create(userProfile);
                applicationUser.UserProfileId = userProfile.Id;
                applicationUser.UserProfile = userProfile;
                repositoryOfApplicationUser.Update(applicationUser);
            }
            return applicationUser;
        }
    }
}
