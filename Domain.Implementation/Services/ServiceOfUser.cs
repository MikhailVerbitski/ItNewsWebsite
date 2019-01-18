using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Implementation.Services
{
    public class ServiceOfUser
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUserEntity> userManager;

        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        //private readonly RepositoryOfUserProfile repositoryOfUserProfile;
        //private readonly RepositoryOfIdentityUserRole repositoryOfRole;

        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfAccount serviceOfAccount;

        public ServiceOfUser(
            ApplicationDbContext context, 
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUserEntity> userManager,
            IMapper mapper, 
            IHostingEnvironment hostingEnvironment
            )
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.userManager = userManager;

            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            //repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            //repositoryOfRole = new RepositoryOfIdentityUserRole(context);

            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfAccount = new ServiceOfAccount(context, userManager, roleManager, hostingEnvironment, mapper);
        }

        public async void EditUser(string applicationUserIdCurrent, UserEditViewModel userEditViewModel)
        {
            var applicationUser = mapper.Map<UserEditViewModel, ApplicationUserEntity>(userEditViewModel);
            applicationUser.Avatar = serviceOfImage.LoadImage("Avatars", applicationUser.Id, userEditViewModel.Avatar, true);
            
            repositoryOfApplicationUser.Update(applicationUser,
                a => a.UserName,
                a => a.LastName,
                a => a.UserName,
                a => a.Email,
                a => a.PhoneNumber,
                a => a.Avatar);

            await serviceOfAccount.ChangePassword(userEditViewModel.ApplicationUserId, userEditViewModel.Password);
            var tasksOfAddsRoles = userEditViewModel
                .Roles
                .Where(a => a.Selected)
                .Select(a => serviceOfAccount.AddUserRole(userEditViewModel.ApplicationUserId, a.Text));
            Task.WaitAll(tasksOfAddsRoles.ToArray());
        }

        public async Task<UserEditViewModel> GetUserEditViewModel(string applicationUserId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var userEditViewModel = mapper.Map<ApplicationUserEntity, UserEditViewModel>(applicationUser);
            var userRoles = await userManager.GetRolesAsync(applicationUser);
            userEditViewModel.Roles = roleManager.Roles.Select(a => new SelectListItem()
            {
                Text = a.Name,
                Value = a.Id,
                Selected = userRoles.Contains(a.Name)
            }).ToList();
            return userEditViewModel;
        }

        public async Task<Tuple<string, string>> GetUserRole(ApplicationUserEntity applicationUserEntity)
        {
            var roles = await userManager.GetRolesAsync(applicationUserEntity);
            if (roles.Contains("admin"))
            {
                return new Tuple<string, string>("admin", "#FF0101");
            }
            else if (roles.Contains("user"))
            {
                return new Tuple<string, string>("user", "#BEA500");
            }
            else
            {
                return new Tuple<string, string>("not found", "#000000");
            }
        }
        public async Task<Tuple<string, string>> GetUserRole(string applicationUserId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            return await GetUserRole(applicationUser);
        }
    }
}
