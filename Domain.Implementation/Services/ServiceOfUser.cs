using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Contracts.Models.ViewModels.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Implementation.Services
{
    public class ServiceOfUser
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUserEntity> userManager;

        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfUserProfile repositoryOfUserProfile;
        private readonly RepositoryOfPost repositoryOfPost;
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
            repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            repositoryOfPost = new RepositoryOfPost(context);
            //repositoryOfRole = new RepositoryOfIdentityUserRole(context);

            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfAccount = new ServiceOfAccount(context, userManager, roleManager, hostingEnvironment, mapper);
        }

        public IEnumerable<UserMiniViewModel> GetUserByProperty(string propetry)
        {
            return GetUsers(a => a.FirstName.Contains(propetry) || a.LastName.Contains(propetry) || a.UserName.Contains(propetry));
        }

        public IEnumerable<UserMiniViewModel> GetUsers(params Expression<Func<ApplicationUserEntity, bool>>[] properties)
        {
            var users = repositoryOfApplicationUser.ReadMany(null);
            foreach (var item in properties)
            {
                users = users.Where(item.Compile());
            }
            var userMiniViewModels = users
                .Select(a => GetUserMiniViewModel(a))
                .ToList();
            return userMiniViewModels;
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

        public UserMiniViewModel GetUserMiniViewModel(ApplicationUserEntity applicationUserPost)
        {
            var userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserPost);
            var role = GetUserRole(applicationUserPost).Result;
            userMiniViewModel.Role = role.Item1;
            userMiniViewModel.RoleColor = role.Item2;
            return userMiniViewModel;
        }

        public UserViewModel GetUserViewModel(string login)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.UserName == login);
            var userProfile = repositoryOfUserProfile.Read(a => a.Id == applicationUser.UserProfileId.Value,
                a => a.Posts,
                a => a.Comments);
            var userViewModel = mapper.Map<ApplicationUserEntity, UserViewModel>(applicationUser);
            var role = GetUserRole(applicationUser).Result;
            userViewModel.Role = role.Item1;
            userViewModel.RoleColor = role.Item2;

            var userMiniViewModel = GetUserMiniViewModel(applicationUser);
            var commentMiniViewModels = userProfile.Comments.Select(a =>
            {
                var comment = mapper.Map<CommentEntity, CommentMiniViewModel>(a);
                comment.AuthorUserMiniViewModel = userMiniViewModel;
                comment.PostHeader = repositoryOfPost.Read(b => b.Id == comment.PostId).Header;
                return comment;
            });
            userViewModel.Comments = commentMiniViewModels.ToList();
            var postMiniViewModels = userProfile.Posts.Select(a =>
            {
                var post = mapper.Map<PostEntity, PostMiniViewModel>(a);
                post.AuthorUserMiniViewModel = userMiniViewModel;
                return post;
            });
            userViewModel.Posts = postMiniViewModels.ToList();
            return userViewModel;
        }
    }
}
