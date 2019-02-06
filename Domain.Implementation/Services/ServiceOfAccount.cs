using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Implementation.Services
{
    public class ServiceOfAccount
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUserEntity> userManager;
        private readonly RoleManager<RoleEntity> roleManager;

        private readonly RepositoryOfRole repositoryOfRole;
        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfUserProfile RepositoryOfUserProfile;
        private readonly RepositoryOfIdentityUserRole repositoryOfIdentityUserRole;

        public ServiceOfImage serviceOfImage { get; set; }

        public ServiceOfAccount(
            ApplicationDbContext context,
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<RoleEntity> roleManager,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper
            )
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;

            repositoryOfRole = new RepositoryOfRole(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            RepositoryOfUserProfile = new RepositoryOfUserProfile(context);
            repositoryOfIdentityUserRole = new RepositoryOfIdentityUserRole(context);

            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
        }

        public ApplicationUserEntity Get(Expression<Func<ApplicationUserEntity, bool>> property)
        {
            return repositoryOfApplicationUser.Read(property);
        }
        public async Task TryToRegistration(string login)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.UserName == login);
            var isRoleExist = await roleManager.RoleExistsAsync("user");
            if (isRoleExist)
            {
                var resultAddRole = await userManager.AddToRoleAsync(applicationUser, "user");
            }
            if(applicationUser.UserProfileId == null)
            {
                if (applicationUser.UserProfileId == null)
                {
                    var userProfile = new UserProfileEntity();
                    userProfile.ApplicationUserId = applicationUser.Id;
                    userProfile = RepositoryOfUserProfile.Create(userProfile);
                    applicationUser.UserProfileId = userProfile.Id;
                    applicationUser.UserProfile = userProfile;
                    applicationUser.Created = System.DateTime.Now;
                }
                repositoryOfApplicationUser.Update(applicationUser);
            }
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
