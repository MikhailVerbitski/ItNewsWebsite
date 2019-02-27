using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
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

        private readonly ServiceOfImage serviceOfImage;

        public ServiceOfAccount(
            IMapper mapper,
            UserManager<ApplicationUserEntity> userManager,
            RoleManager<RoleEntity> roleManager,
            RepositoryOfRole repositoryOfRole,
            RepositoryOfApplicationUser repositoryOfApplicationUser,
            RepositoryOfUserProfile RepositoryOfUserProfile,
            RepositoryOfIdentityUserRole repositoryOfIdentityUserRole,
            ServiceOfImage serviceOfImage
            )
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.repositoryOfRole = repositoryOfRole;
            this.repositoryOfApplicationUser = repositoryOfApplicationUser;
            this.RepositoryOfUserProfile = RepositoryOfUserProfile;
            this.repositoryOfIdentityUserRole = repositoryOfIdentityUserRole;
            this.serviceOfImage = serviceOfImage;
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
                await userManager.AddToRoleAsync(applicationUser, "user");
            }
            if(applicationUser.UserProfileId == null)
            {
                var userProfile = new UserProfileEntity();
                userProfile.ApplicationUserId = applicationUser.Id;
                userProfile = RepositoryOfUserProfile.Create(userProfile);
                applicationUser.UserProfileId = userProfile.Id;
                applicationUser.UserProfile = userProfile;
                applicationUser.Created = System.DateTime.Now;
                repositoryOfApplicationUser.Update(applicationUser);
            }
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
        public void SendEmailAsync(string email, string subject, string message)
        {
            using (SmtpClient client = new SmtpClient())
            {
                using(MailMessage mail = new MailMessage("itnews68@gmail.com", email))
                {
                    mail.Subject = subject;
                    mail.Body = message;

                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("itnews68@gmail.com", "ItNews123456");
                    client.Send(mail);
                }
            }
        }
    }
}
