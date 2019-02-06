using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Contracts.Models.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Implementation.Services
{
    public class ServiceOfUser
    {
        private readonly IMapper mapper;

        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfUserProfile repositoryOfUserProfile;
        private readonly RepositoryOfPost repositoryOfPost;

        public ServiceOfImage serviceOfImage { get; set; }
        public ServiceOfAccount serviceOfAccount { get; set; }
        public ServiceOfComment serviceOfComment { get; set; }
        public ServiceOfPost serviceOfPost { get; set; }

        public ServiceOfUser(
            ApplicationDbContext context,
            IMapper mapper,
            ServiceOfImage serviceOfImage,
            ServiceOfAccount serviceOfAccount,
            ServiceOfComment serviceOfComment,
            ServiceOfPost serviceOfPost
            )
        {
            this.mapper = mapper;

            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            repositoryOfPost = new RepositoryOfPost(context);

            this.serviceOfImage = serviceOfImage;
            this.serviceOfAccount = serviceOfAccount;
            this.serviceOfComment = serviceOfComment;
            this.serviceOfPost = serviceOfPost;
        }

        public IEnumerable<UserMiniViewModel> GetUserByProperty(string propetry)
        {
            return GetUsers(a => a.FirstName.Contains(propetry) || a.LastName.Contains(propetry) || a.UserName.Contains(propetry));
        }

        public IEnumerable<UserMiniViewModel> GetUsers(params Expression<Func<ApplicationUserEntity, bool>>[] whereProperties)
        {
            return repositoryOfApplicationUser
                .ReadMany(whereProperties)
                .Select(a => GetUserMiniViewModel(a))
                .ToList();
        }
        public string ChangeUserImage(string applicationUserCurrent, UserImage image)
        {
            var path = serviceOfImage.LoadImage(applicationUserCurrent, image);
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserCurrent);
            applicationUser.Avatar = path;
            repositoryOfApplicationUser.Update(applicationUser, a => a.Avatar);
            return path;
        }
        public async Task Update(string applicationUserIdCurrent, UserUpdateViewModel userUpdateViewModel)
        {
            var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if(await serviceOfAccount.IsThereAccess(applicationUserCurrent, userUpdateViewModel.ApplicationUserId))
            {
                var applicationUser = mapper.Map<UserUpdateViewModel, ApplicationUserEntity>(userUpdateViewModel);
                repositoryOfApplicationUser.Update(applicationUser,
                    a => a.FirstName,
                    a => a.LastName,
                    a => a.UserName,
                    a => a.Email);

                if (userUpdateViewModel.Password != null && userUpdateViewModel.Password != "")
                {
                    await serviceOfAccount.ChangePassword(applicationUser.Id, userUpdateViewModel.Password);
                }
                //var tasksOfAddsRoles = userEditViewModel
                //    .Roles
                //    .Where(a => a.Selected)
                //    .Select(a => serviceOfAccount.AddUserRole(userEditViewModel.ApplicationUserId, a.Text));
                //Task.WaitAll(tasksOfAddsRoles.ToArray());
            }
        }

        public async Task<Tuple<string, string>> GetUserRole(ApplicationUserEntity applicationUserEntity)
        {
            var roles = await serviceOfAccount.GetUserRoles(applicationUserEntity);
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

        public UserMiniViewModel GetUserMiniViewModel(ApplicationUserEntity applicationUserPost)
        {
            var userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserPost);
            var role = GetUserRole(applicationUserPost).Result;
            userMiniViewModel.Role = role.Item1;
            userMiniViewModel.RoleColor = role.Item2;
            return userMiniViewModel;
        }
        public async Task<UserViewModel> GetUserViewModel(string applicationUserIdCurrent, string login)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.UserName == login);
            var applicationUserCurrent = (applicationUserIdCurrent == null) 
                ? null 
                : (applicationUserIdCurrent == applicationUser.Id) 
                    ? applicationUser 
                    : repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            var userProfile = repositoryOfUserProfile.Read(a => a.Id == applicationUser.UserProfileId.Value,
                a => a.Posts,
                a => a.Comments);
            var userViewModel = mapper.Map<ApplicationUserEntity, UserViewModel>(applicationUser);
            var role = GetUserRole(applicationUser).Result;
            userViewModel.Role = role.Item1;
            userViewModel.RoleColor = role.Item2;
            userViewModel.Comments = userProfile.Comments.Select(a => serviceOfComment.GetViewModelWithProperty<CommentMiniViewModel>(a, applicationUserCurrent)).ToList();
            userViewModel.Posts = userProfile.Posts.Select(a => serviceOfPost.GetViewModelWithProperty<PostCompactViewModel>(a, applicationUserCurrent)).ToList();
            userViewModel.IsCurrentUser = await serviceOfAccount.IsThereAccess(applicationUserCurrent, applicationUser.Id);
            return userViewModel;
        }
    }
}
