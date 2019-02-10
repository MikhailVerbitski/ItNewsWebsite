﻿using AutoMapper;
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
        ApplicationDbContext context;

        private readonly RepositoryOfIdentityUserRole repositoryOfIdentityUserRole;
        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfUserProfile repositoryOfUserProfile;
        private readonly RepositoryOfPost repositoryOfPost;

        public ServiceOfImage serviceOfImage { get; set; }
        public ServiceOfAccount serviceOfAccount { get; set; }
        public ServiceOfComment serviceOfComment { get; set; }
        public ServiceOfPost serviceOfPost { get; set; }
        public ServiceOfRole serviceOfRole { get; set; }

        public ServiceOfUser(
            ApplicationDbContext context,
            IMapper mapper,
            ServiceOfImage serviceOfImage,
            ServiceOfAccount serviceOfAccount,
            ServiceOfComment serviceOfComment,
            ServiceOfPost serviceOfPost, 
            ServiceOfRole serviceOfRole
            )
        {
            this.mapper = mapper;
            this.context = context;

            repositoryOfIdentityUserRole = new RepositoryOfIdentityUserRole(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            repositoryOfPost = new RepositoryOfPost(context);

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
        public async Task Update(string applicationUserIdCurrent, UserUpdateViewModel userUpdateViewModel)
        {
            var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if(await serviceOfRole.IsThereAccess(new[] { 3 }, applicationUserCurrent, userUpdateViewModel.ApplicationUserId, true))
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
                if(await serviceOfRole.GetUserPriority(applicationUserCurrent) == 3)
                {
                    await serviceOfRole.ChangeRole(applicationUser, userUpdateViewModel.Role);
                }
            }
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
            userViewModel.IsCurrentUser = await serviceOfRole.IsThereAccess(new[] { 3 }, applicationUserCurrent, applicationUser.Id, true);
            return userViewModel;
        }
    }
}
