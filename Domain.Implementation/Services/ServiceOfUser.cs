﻿using AutoMapper;
using Data.Contracts;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Search.Implementation;
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
        private readonly IRepository<IdentityUserRole<string>> repositoryOfIdentityUserRole;
        private readonly IRepository<ApplicationUserEntity> repositoryOfApplicationUser;
        private readonly IRepository<UserProfileEntity> repositoryOfUserProfile;
        private readonly IRepository<IdentityUserClaim<string>> repositoryOfUserClaim;
        private readonly IRepository<PostEntity> repositoryOfPost;
        private readonly IRepository<RoleEntity> repositoryOfRole;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfSearch serviceOfSearch;

        public Dictionary<string, Func<string, string, UserClaim>> Climes { get; set; } = new Dictionary<string, Func<string, string, UserClaim>>(new[] {
            new KeyValuePair<string, Func<string, string, UserClaim>>("blocked", (u,v) => new UserClaim(){ UserId = u, ClaimValue = ((v == string.Empty) ? $"your account has been suspended due to: " : v), ClaimType = "blocked" })
        });

        public ServiceOfUser(
            ApplicationDbContext context,
            UserManager<ApplicationUserEntity> userManager,
            IMapper mapper,
            ServiceOfImage serviceOfImage,
            ServiceOfSearch serviceOfSearch,
            IRepository<IdentityUserRole<string>> repositoryOfIdentityUserRole,
            IRepository<ApplicationUserEntity> repositoryOfApplicationUser,
            IRepository<UserProfileEntity> repositoryOfUserProfile,
            IRepository<IdentityUserClaim<string>> repositoryOfUserClaim,
            IRepository<PostEntity> repositoryOfPost,
            IRepository<RoleEntity> repositoryOfRole
            )
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.repositoryOfIdentityUserRole = repositoryOfIdentityUserRole;
            this.repositoryOfApplicationUser = repositoryOfApplicationUser;
            this.repositoryOfUserProfile = repositoryOfUserProfile;
            this.repositoryOfUserClaim = repositoryOfUserClaim;
            this.repositoryOfPost = repositoryOfPost;
            this.repositoryOfRole = repositoryOfRole;
            this.serviceOfImage = serviceOfImage;
            this.serviceOfSearch = serviceOfSearch;
        }

        public IEnumerable<UserMiniViewModel> Search(string propetry, int? skip, int? take)
        {
            var ids = serviceOfSearch.SearchUsers(propetry, (skip != null) ? skip.Value : 0, (take != null) ? take.Value : 0);
            var result = ids
                .Select(b => repositoryOfApplicationUser.Read(a => a.Id == b, a=> a.UserProfile))
                .Select(a => GetUserMiniViewModel(a))
                .ToList();
            return result;
        }
        public IEnumerable<UserMiniViewModel> GetUsers(params Expression<Func<ApplicationUserEntity, bool>>[] whereProperties)
        {
            return repositoryOfApplicationUser
                .ReadMany(whereProperties, null)
                .Select(a => GetUserMiniViewModel(a))
                .ToList();
        }
        public string ChangeUserImage(string host, string applicationUserCurrent, UserImage image)
        {
            var path = serviceOfImage.LoadImage(host, applicationUserCurrent, image);
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
                    ChangePassword(applicationUser.Id, userViewModel.Password);
                }
                if(await GetUserPriority(applicationUserCurrent) == 3)
                {
                    await ChangeRole(applicationUser, userViewModel.Role);
                }
                if(userViewModel.UserClaims != null)
                {
                    var userClaims = GetUserClaim(userViewModel.ApplicationUserId);
                    userClaims.Except(userViewModel.UserClaims, new UserClaim()).ToList().ForEach(a => RemoveUserClaim(a));
                    userViewModel.UserClaims.Except(userClaims, new UserClaim()).ToList().ForEach(a => SetUserClime(applicationUser.Id, a.ClaimType, a.ClaimValue));
                }
            }
        }
        public void ChangePassword(string applicationUserId, string newPassword)
        {
            var lastApplicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            userManager.ChangePasswordAsync(lastApplicationUser, lastApplicationUser.PasswordHash, newPassword);
        }
        public UserClaim GetUserClaim(string ApplicationUserId, string ClaimType)
        {
            return mapper.Map<IdentityUserClaim<string>, UserClaim>(repositoryOfUserClaim.Read(a => a.UserId == ApplicationUserId && a.ClaimType == ClaimType));
        }
        public List<UserClaim> GetUserClaim(string ApplicationUserId)
        {
            return mapper.Map<IEnumerable< IdentityUserClaim<string>>, IEnumerable< UserClaim>>(repositoryOfUserClaim.ReadMany(new Expression<Func<IdentityUserClaim<string>, bool>>[] { a => a.UserId == ApplicationUserId }, null)).ToList();
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
            userMiniViewModel.Role = GetUserRole(applicationUserPost).Result;
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
            userViewModel.Role = await GetUserRole(applicationUser);
            userViewModel.UserClaims = GetUserClaim(applicationUser.Id);
            userViewModel.IsCurrentUser = await IsThereAccess(new[] { 3 }, applicationUserCurrent, applicationUser.Id, true);
            if(await IsThereAccess(new[] { 3 }, applicationUserCurrent, applicationUser.Id, false))
            {
                userViewModel.AllClaims = Climes.Select(a => a.Value(string.Empty, string.Empty)).ToList();
                userViewModel.AllRoles = new[] { userViewModel.Role }.Union(GetRoles().Except(new[] { userViewModel.Role }, new UserRole())).ToList();
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
            var roles = repositoryOfRole.ReadMany().ToList();
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
        public async Task<UserRole> GetUserRole(ApplicationUserEntity applicationUser)
        {
            var roles = repositoryOfRole.ReadMany().ToList();
            var role = (await userManager.GetRolesAsync(applicationUser)).Select(a => roles.Where(b => b.Name == a).FirstOrDefault()).FirstOrDefault();
            return mapper.Map<RoleEntity, UserRole>(role);
        }
        public List<UserRole> GetRoles()
        {
            return mapper.Map<IEnumerable<RoleEntity>, IEnumerable<UserRole>>(repositoryOfRole.ReadMany()).ToList();
        }
        public async Task ChangeRole(ApplicationUserEntity applicationUser, UserRole newRole)
        {
            var roleName = (await userManager.GetRolesAsync(applicationUser)).FirstOrDefault();
            var role = repositoryOfRole.Read(a => a.Name == roleName);
            if (newRole.Name != role.Name)
            {
                repositoryOfIdentityUserRole.Delete(new IdentityUserRole<string> { UserId = applicationUser.Id, RoleId = role.Id });
                repositoryOfIdentityUserRole.Create(new IdentityUserRole<string> { UserId = applicationUser.Id, RoleId = newRole.Id });
            }
        }
        public async Task Delete(string applicationUserCurrentId, int userProfileId)
        {
            var currentUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserCurrentId);
            var user = (currentUser.UserProfileId != userProfileId) ? repositoryOfApplicationUser.Read(a => a.UserProfileId == userProfileId) : currentUser;
            if (await IsThereAccess(new[] { 3 }, currentUser, user.Id, true))
            {
                repositoryOfApplicationUser.Delete(user);
            }
        }
        private CommentMiniViewModel GetCommentMiniViewModel(CommentEntity commentEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var commentViewModel = mapper.Map<CommentEntity, CommentMiniViewModel>(commentEntity);
            var applicationUserForComment = repositoryOfUserProfile.Read(a => a.Id == commentEntity.UserProfileId, a => a.ApplicationUser).ApplicationUser;
            UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserForComment);
            commentViewModel.PostHeader = commentEntity.Post == null ? repositoryOfPost.Read(a => a.Id == commentEntity.PostId).Header : commentEntity.Post.Header;
            commentViewModel.BelongsToUser = (applicationUserCurrent == null) ? false : applicationUserCurrent.UserProfileId == commentEntity.UserProfileId;
            commentViewModel.AuthorUserMiniViewModel = GetUserMiniViewModel(applicationUserForComment);
            return commentViewModel;
        }
    }
}
