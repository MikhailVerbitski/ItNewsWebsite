﻿using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Contracts.Models.ViewModels.User;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUserEntity> userManager;

        private readonly RepositoryOfIdentityUserRole repositoryOfIdentityUserRole;
        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfUserProfile repositoryOfUserProfile;
        private readonly RepositoryOfUserClaim repositoryOfUserClaim;
        private readonly RepositoryOfPost repositoryOfPost;
        private readonly RepositoryOfRole repositoryOfRole;

        public ServiceOfImage serviceOfImage { get; set; }
        public ServiceOfAccount serviceOfAccount { get; set; }
        public ServiceOfComment serviceOfComment { get; set; }
        public ServiceOfPost serviceOfPost { get; set; }
        public ServiceOfRole serviceOfRole { get; set; }

        public Dictionary<string, Func<string, string, UserClaim>> Climes { get; set; } = new Dictionary<string, Func<string, string, UserClaim>>(new[] {
            new KeyValuePair<string, Func<string, string, UserClaim>>("blocked", (u,v) => new UserClaim(){ UserId = u, ClaimValue = ((v == string.Empty) ? $"your account has been suspended due to: " : v), ClaimType = "blocked" })
        });

        public ServiceOfUser(
            ApplicationDbContext context,
            UserManager<ApplicationUserEntity> userManager,
            IMapper mapper,
            ServiceOfImage serviceOfImage,
            ServiceOfAccount serviceOfAccount,
            ServiceOfComment serviceOfComment,
            ServiceOfPost serviceOfPost, 
            ServiceOfRole serviceOfRole
            )
        {
            this.mapper = mapper;
            this.userManager = userManager;

            repositoryOfIdentityUserRole = new RepositoryOfIdentityUserRole(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            repositoryOfUserClaim = new RepositoryOfUserClaim(context);
            repositoryOfPost = new RepositoryOfPost(context);
            repositoryOfRole = new RepositoryOfRole(context);

            this.serviceOfImage = serviceOfImage;
            this.serviceOfAccount = serviceOfAccount;
            this.serviceOfComment = serviceOfComment;
            this.serviceOfPost = serviceOfPost;
            this.serviceOfRole = serviceOfRole;
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
        public async Task Update(string applicationUserIdCurrent, UserViewModel userViewModel)
        {
            var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if(await IsThereAccess(new[] { 3 }, applicationUserCurrent, userViewModel.ApplicationUserId, true))
            {
                var applicationUser = mapper.Map<UserViewModel, ApplicationUserEntity>(userViewModel);
                repositoryOfApplicationUser.Update(applicationUser,
                    a => a.FirstName,
                    a => a.LastName,
                    a => a.UserName,
                    a => a.Email);

                if (userViewModel.Password != null && userViewModel.Password != "")
                {
                    await serviceOfAccount.ChangePassword(applicationUser.Id, userViewModel.Password);
                }
                if(await GetUserPriority(applicationUserCurrent) == 3)
                {
                    await serviceOfRole.ChangeRole(applicationUser, userViewModel.Role);
                }
                if(userViewModel.UserClaims != null)
                {
                    var userClaims = GetUserClaim(userViewModel.ApplicationUserId);
                    userClaims.Except(userViewModel.UserClaims, new UserClaim()).ToList().ForEach(a => RemoveUserClaim(a));
                    userViewModel.UserClaims.Except(userClaims, new UserClaim()).ToList().ForEach(a => SetUserClime(applicationUser.Id, a.ClaimType, a.ClaimValue));
                }
            }
        }
        public UserClaim GetUserClaim(string ApplicationUserId, string ClaimType)
        {
            return mapper.Map<IdentityUserClaim<string>, UserClaim>(repositoryOfUserClaim.Read(a => a.UserId == ApplicationUserId && a.ClaimType == ClaimType));
        }
        public List<UserClaim> GetUserClaim(string ApplicationUserId)
        {
            return mapper.Map<IEnumerable< IdentityUserClaim<string>>, IEnumerable< UserClaim>>(repositoryOfUserClaim.ReadMany(new Expression<Func<IdentityUserClaim<string>, bool>>[] { a => a.UserId == ApplicationUserId })).ToList();
        }
        public void SetUserClime(string userId, string name, string value)
        {
            var clime = repositoryOfUserClaim.Read(a => a.UserId == userId && a.ClaimType == name);
            if(clime != null)
            {
                return;
            }
            var userClime = (Climes.Any(a => a.Key == name))
                ? Climes.GetValueOrDefault(name)(userId, value)
                : new UserClaim() { UserId = userId, ClaimType = name, ClaimValue = value };
            clime = mapper.Map<UserClaim, IdentityUserClaim<string>>(userClime);
            repositoryOfUserClaim.Create(clime);
        }
        public void RemoveUserClaim(UserClaim userClaim)
        {
            var claim = mapper.Map<UserClaim, IdentityUserClaim<string>>(userClaim);
            repositoryOfUserClaim.Delete(claim);
        }
        public UserMiniViewModel GetUserMiniViewModel(ApplicationUserEntity applicationUserPost)
        {
            var userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserPost);
            userMiniViewModel.Role = serviceOfRole.GetUserRole(applicationUserPost).Result;
            return userMiniViewModel;
        }
        public async Task<UserViewModel> GetUserViewModel(string applicationUserIdCurrent, int userProfileId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.UserProfileId == userProfileId);
            var applicationUserCurrent = (applicationUserIdCurrent == null)
                ? null
                : (applicationUserIdCurrent == applicationUser.Id)
                    ? applicationUser
                    : repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            var userProfile = repositoryOfUserProfile.Read(a => a.Id == applicationUser.UserProfileId.Value,
                a => a.Posts,
                a => a.Comments);
            var userViewModel = mapper.Map<ApplicationUserEntity, UserViewModel>(applicationUser);
            userViewModel.Role = await serviceOfRole.GetUserRole(applicationUser);
            userViewModel.Comments = userProfile.Comments.Select(a => serviceOfComment.GetViewModelWithProperty<CommentMiniViewModel>(a, applicationUserCurrent)).ToList();
            userViewModel.Posts = userProfile.Posts.Select(a => serviceOfPost.GetViewModelWithProperty(nameof(PostMiniViewModel), a, applicationUserCurrent) as PostMiniViewModel).ToList();
            userViewModel.UserClaims = GetUserClaim(applicationUser.Id);
            userViewModel.IsCurrentUser = await IsThereAccess(new[] { 3 }, applicationUserCurrent, applicationUser.Id, true);
            if(await IsThereAccess(new[] { 3 }, applicationUserCurrent, applicationUser.Id, false))
            {
                userViewModel.AllClaims = Climes.Select(a => a.Value(string.Empty, string.Empty)).ToList();
                userViewModel.AllRoles = new[] { userViewModel.Role }.Union(serviceOfRole.GetRoles().Except(new[] { userViewModel.Role }, new UserRole())).ToList();
            }
            return userViewModel;
        }
        public async Task<DataAboutCurrentUser> GetUserData(string userId)
        {
            DataAboutCurrentUser data = new DataAboutCurrentUser();
            var user = repositoryOfApplicationUser.Read(a => a.Id == userId);
            data.Priority = await GetUserPriority(user);
            data.Login = user.UserName;
            data.UserProfileId = user.UserProfileId.Value;
            data.claims = GetUserClaim(user.Id);
            return data;
        }
        public async Task<int> GetUserPriority(string applicationUserId)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            return await GetUserPriority(applicationUser);
        }
        public async Task<int> GetUserPriority(ApplicationUserEntity applicationUser)
        {
            if (!applicationUser.EmailConfirmed)
            {
                return 0;
            }
            var roles = repositoryOfRole.ReadMany(null).ToList();
            var IsBlocked = GetUserClaim(applicationUser.Id, "blocked");
            if (IsBlocked != null)
            {
                return 1;
            }
            var role = (await userManager.GetRolesAsync(applicationUser)).Select(a => roles.FirstOrDefault(b => b.Name == a)).FirstOrDefault();
            return role.Priority;
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
            var userCurrentPriority = await GetUserPriority(applicationUserCurrent);
            if (prioritiesWithResolution.Contains(userCurrentPriority))
            {
                return true;
            }
            return false;
        }
    }
}
