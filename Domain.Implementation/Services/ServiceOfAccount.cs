using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Account;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Implementation.Services
{
    public class ServiceOfAccount
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUserEntity> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfUserProfile RepositoryOfUserProfile;
        private readonly RepositoryOfIdentityUserRole repositoryOfIdentityUserRole;

        private readonly ServiceOfImage serviceOfImage;

        public ServiceOfAccount(
            ApplicationDbContext context,
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<IdentityRole> roleManager,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;

            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            RepositoryOfUserProfile = new RepositoryOfUserProfile(context);
            repositoryOfIdentityUserRole = new RepositoryOfIdentityUserRole(context);

            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
        }

        public async Task<IdentityResult> TryToRegistration(RegisterViewModel registerViewModel)
        {
            ApplicationUserEntity applicationUser = mapper.Map<RegisterViewModel, ApplicationUserEntity>(registerViewModel);
            var result =  await userManager.CreateAsync(applicationUser, registerViewModel.Password);
            if(result.Succeeded)
            {
                var isRoleExist = await roleManager.RoleExistsAsync("user");
                if (isRoleExist)
                {
                    var resultAddRole = await userManager.AddToRoleAsync(applicationUser, "user");
                }
                if (applicationUser.UserProfileId == null)
                {
                    var userProfile = new UserProfileEntity();
                    userProfile.ApplicationUserId = applicationUser.Id;
                    userProfile = RepositoryOfUserProfile.Create(userProfile);
                    applicationUser.UserProfileId = userProfile.Id;
                    applicationUser.UserProfile = userProfile;
                    repositoryOfApplicationUser.Update(applicationUser);
                }
                if (registerViewModel.Avatar != null)
                {
                    var pathAvatar = serviceOfImage.LoadImage("Avatars", applicationUser.Id, registerViewModel.Avatar);
                    applicationUser.Avatar = pathAvatar;
                    repositoryOfApplicationUser.Update(applicationUser);
                }
            }
            return result;
        }

        public async Task ChangePassword(string applicationUserId, string newPassword)
        {
            var lastApplicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            await userManager.ChangePasswordAsync(lastApplicationUser, lastApplicationUser.PasswordHash, newPassword);
        }

        public async Task<bool> AddUserRole(string applicationUserId, params string[] roles)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var result = await userManager.AddToRolesAsync(applicationUser, roles);
            return result.Succeeded;
        }
        public async Task<IdentityResult> RemoveUserRole(string applicationUserId, params string[] roles)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var result = await userManager.RemoveFromRolesAsync(applicationUser, roles);
            return result;
        }
    }
}
