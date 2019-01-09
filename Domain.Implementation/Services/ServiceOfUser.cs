using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Account;
using Domain.Contracts.Models.ViewModels.User;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Security.Cryptography;

namespace Domain.Implementation.Services
{
    public class ServiceOfUser
    {
        ApplicationDbContext context;

        private readonly IMapper mapper;

        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfUserProfile repositoryOfUserProfile;
        private readonly RepositoryOfRole repositoryOfRole;

        private readonly ServiceOfImage serviceOfImage;

        public ServiceOfUser(ApplicationDbContext context, IMapper mapper, IHostingEnvironment hostingEnvironment)
        {
            this.context = context;

            this.mapper = mapper;

            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            repositoryOfRole = new RepositoryOfRole(context);

            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
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
            if(registerViewModel.Avatar != null)
            {
                var pathAvatar = serviceOfImage.LoadImage("Avatars", applicationUser.Id, registerViewModel.Avatar);
                applicationUser.Avatar = pathAvatar;
                repositoryOfApplicationUser.Update(applicationUser);
            }

            var test = repositoryOfApplicationUser.Read(a => a.Id == applicationUser.Id);

            return applicationUser;
        }

        public void EditUser(UserEditViewModel userEditViewModel)
        {
            var applicationUser = mapper.Map<UserEditViewModel, ApplicationUserEntity>(userEditViewModel);
            var roleId = repositoryOfRole.Read(a => a.Id == Int32.Parse(userEditViewModel.Role)).Id;
            applicationUser.Avatar = serviceOfImage.LoadImage("Avatars", applicationUser.Id, userEditViewModel.Avatar, true);
            applicationUser.RoleId = roleId;
            applicationUser.PasswordHash = GetHashPassword(userEditViewModel.Password);

            repositoryOfApplicationUser.Update(applicationUser);
        }
        private string GetHashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        public UserEditViewModel GetUserEditViewModel(string applicationUserId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var userEditViewModel = mapper.Map<ApplicationUserEntity, UserEditViewModel>(applicationUser);
            return userEditViewModel;
        }
    }
}
