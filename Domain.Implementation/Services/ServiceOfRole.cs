using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Implementation.Services
{
    public class ServiceOfRole
    {
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUserEntity> userManager;
        private readonly RoleManager<RoleEntity> roleManager;

        private readonly RepositoryOfRole repositoryOfRole;
        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfIdentityUserRole repositoryOfIdentityUserRole;

        public ServiceOfRole(ApplicationDbContext context, UserManager<ApplicationUserEntity> userManager, RoleManager<RoleEntity> roleManager, IMapper mapper)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;

            repositoryOfRole = new RepositoryOfRole(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfIdentityUserRole = new RepositoryOfIdentityUserRole(context);
        }
        
        public async Task<int> GetUserPriority(string applicationUserId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            return await GetUserPriority(applicationUser);
        }
        public async Task<int> GetUserPriority(ApplicationUserEntity applicationUser)
        {
            var roles = repositoryOfRole.ReadMany(null).ToList();
            var role = (await userManager.GetRolesAsync(applicationUser)).Select(a => roles.Where(b => b.Name == a).FirstOrDefault()).FirstOrDefault();
            return role.Priority;
        }
        public async Task<UserRole> GetUserRole(string applicationUserId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            return await GetUserRole(applicationUser);
        }
        public async Task<UserRole> GetUserRole(ApplicationUserEntity applicationUser)
        {
            var roles = repositoryOfRole.ReadMany(null).ToList();
            var role = (await userManager.GetRolesAsync(applicationUser)).Select(a => roles.Where(b => b.Name == a).FirstOrDefault()).FirstOrDefault();
            return mapper.Map<RoleEntity, UserRole>(role);
        }
        public async Task<bool> IsThereAccess(int[] prioritiesWithResolution, ApplicationUserEntity applicationUserCurrent, string applicationUserIdRequest, bool allowOwner)
        {
            if (applicationUserCurrent == null)
            {
                return false;
            }
            if (allowOwner && applicationUserCurrent.Id == applicationUserIdRequest)
            {
                return true;
            }
            var roles = repositoryOfRole.ReadMany(null).ToList();
            var userRole = (await userManager.GetRolesAsync(applicationUserCurrent)).Select(a => roles.FirstOrDefault(b => b.Name == a)).FirstOrDefault();
            if (prioritiesWithResolution.Contains(userRole.Priority))
            {
                return true;
            }
            return false;
        }
        public List<UserRole> GetRoles()
        {
            return mapper.Map<IEnumerable<RoleEntity>, IEnumerable<UserRole>>(repositoryOfRole.ReadMany(null)).ToList();
        }
        public async Task ChangeRole(ApplicationUserEntity applicationUser, UserRole newRole)
        {
            var roleName = (await userManager.GetRolesAsync(applicationUser)).FirstOrDefault();
            var role = repositoryOfRole.Read(a => a.Name == roleName);
            repositoryOfIdentityUserRole.Delete(new IdentityUserRole<string> { UserId = applicationUser.Id, RoleId = role.Id });
            repositoryOfIdentityUserRole.Create(new IdentityUserRole<string> { UserId = applicationUser.Id, RoleId = newRole.Id });
        }
    }
}
