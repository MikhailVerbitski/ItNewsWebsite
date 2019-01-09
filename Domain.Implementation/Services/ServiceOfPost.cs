﻿using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Contracts.Models.ViewModels.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Domain.Implementation.Services
{
    public class ServiceOfPost
    {
        private readonly IMapper mapper;

        private readonly RepositoryOfPost repositoryOfPost;
        private readonly RepositoryOfPostRating repositoryOfPostRating;
        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfTag repositoryOfTag;
        private readonly RepositoryOfPostTag repositoryOfPostTag;
        private readonly RepositoryOfSection repositoryOfSection;
        private readonly RepositoryOfUserProfile repositoryOfUserProfile;
        private readonly RepositoryOfCommentLike repositoryOfCommentLike;

        private readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfImage serviceOfImage;

        public ServiceOfPost(ApplicationDbContext context, IMapper mapper, IHostingEnvironment hostingEnvironment)
        {
            this.mapper = mapper;

            repositoryOfPost = new RepositoryOfPost(context);
            repositoryOfPostRating = new RepositoryOfPostRating(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfTag = new RepositoryOfTag(context);
            repositoryOfPostTag = new RepositoryOfPostTag(context);
            repositoryOfSection = new RepositoryOfSection(context);
            repositoryOfUserProfile = new RepositoryOfUserProfile(context);
            repositoryOfCommentLike = new RepositoryOfCommentLike(context);

            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfComment = new ServiceOfComment(context, mapper);
        }

        public PostCreateEditViewModel CreateNotFinished(string applicationUserId)
        {
            var postEntity = new PostEntity();
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            postEntity.UserProfileId = applicationUser.UserProfileId;
            postEntity.IsFinished = false;
            postEntity = repositoryOfPost.Create(postEntity);
            var postViewModel = mapper.Map<PostEntity, PostCreateEditViewModel>(postEntity);
            return postViewModel;
        }
        public PostViewModel CreateFinished(PostCreateEditViewModel postCreateEditViewModel, bool isReturnViewModel = false)
        {
            var postEntity = mapper.Map<PostCreateEditViewModel, PostEntity>(postCreateEditViewModel);
            
            var section = repositoryOfSection.Read(a => a.Id == Int32.Parse(postCreateEditViewModel.Section));
            postEntity.Section = section;
            postEntity.SectionId = section.Id;
            
            var tagsOfPost = postCreateEditViewModel.Tags.Split(" ").Distinct();
            var tags = tagsOfPost.Select(a => {
                var tag = repositoryOfTag.Read(b => b.Name == a);
                if(tag == null)
                {
                    tag = repositoryOfTag.Create(new TagEntity()
                    {
                        Name = a,
                    });
                }
                return tag;
            }).ToList();
            var postTagEntities = tags
                .Select(a => repositoryOfPostTag.Create(new PostTagEntity()
                {
                    PostId = postEntity.Id,
                    TagId = a.Id,
                    Tag = a,
                }))
                .ToList();
            
            postEntity.Tags = postTagEntities;
            postEntity.IsFinished = true;
            
            repositoryOfPost.Update(postEntity);
            if(isReturnViewModel)
            {
                var postViewModel = mapper.Map<PostEntity, PostViewModel>(postEntity);
                return postViewModel;
            }
            return null;
        }

        public void AddImage(int postId, params IFormFile[] images)
        {
            if(images.Length == 0)
            {
                return;
            }
            var imageEntities = images
                .Select(a => serviceOfImage.CreateImageForPost(postId, a))
                .ToList();
        }

        public IEnumerable<TPostViewModel> Get<TPostViewModel>(string ApplicationUserId = null, bool postIsFinished = true) where TPostViewModel : class
        {
            ApplicationUserEntity applicationUser = null;
            IEnumerable<PostEntity> posts;
            if (ApplicationUserId != null)
            {
                applicationUser = repositoryOfApplicationUser.Read(a => a.Id == ApplicationUserId);
                posts = repositoryOfPost.ReadMany(a => a.UserProfileId == applicationUser.UserProfileId, 
                    a => a.Tags, 
                    a => a.Comments, 
                    a => a.Section, 
                    a => a.UserProfile);
            }
            else
            {
                posts = repositoryOfPost.ReadMany(null , a => a.Comments, a => a.Tags, a => a.Section, a => a.UserProfile);
            }
            posts = posts.Where(a => a.IsFinished == postIsFinished);

            List<TPostViewModel> postsViewModels = new List<TPostViewModel>();

            var propertyTags = typeof(TPostViewModel).GetProperty("Tags");
            var propertyUserMiniViewModel = typeof(TPostViewModel).GetProperty("AuthorUserMiniViewModel");
            var propertyComments = typeof(TPostViewModel).GetProperty("Comments");

            foreach (var item in posts)
            {
                var postViewModel = mapper.Map<PostEntity, TPostViewModel>(item);

                if (propertyTags != null)
                {
                    var tags = item.Tags.Select(a => repositoryOfTag.Read(b => b.Id == a.TagId).Name);
                    propertyTags.SetValue(postViewModel, tags);
                }
                if (propertyUserMiniViewModel != null)
                {
                    var applicationUserEntity = repositoryOfApplicationUser.Read(a => a.Id == item.UserProfile.ApplicationUserId);
                    UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserEntity);
                    propertyUserMiniViewModel.SetValue(postViewModel, userMiniViewModel);
                }
                if (propertyComments != null)
                {
                    var comentsViewModels = new List<CommentViewModel>();

                    foreach (var commentEntity in item.Comments)
                    {
                        var applicationUserForComment = repositoryOfUserProfile.Read(a => a.Id == commentEntity.UserProfileId, a => a.ApplicationUser).ApplicationUser;
                        UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserForComment);
                        var commentViewModel = mapper.Map<CommentEntity, CommentViewModel>(commentEntity);
                        commentViewModel.AuthorUserMiniViewModel = userMiniViewModel;

                        var commentLikeEntity = repositoryOfCommentLike.Read(a => a.CommentId == commentEntity.Id);
                        commentViewModel.IsUserLiked = (commentLikeEntity == null)
                            ? false
                            : commentLikeEntity.UserProfileId == applicationUser.UserProfileId;

                        comentsViewModels.Add(commentViewModel);
                    }
                    propertyComments.SetValue(postViewModel, comentsViewModels);
                }
                postsViewModels.Add(postViewModel);
            }
            return postsViewModels;
        }

        public TPostViewModel Get<TPostViewModel>(string currentApplicationUserId, int postId) where TPostViewModel : class
        {
            var postEntity = repositoryOfPost.Read(a => a.Id == postId, 
                a => a.Tags, 
                a => a.Comments, 
                a => a.Section, 
                a => a.UserProfile);
            ApplicationUserEntity applicationUser = repositoryOfApplicationUser.Read(a => a.UserProfileId == postEntity.UserProfileId);

            TPostViewModel postViewModel = mapper.Map<PostEntity, TPostViewModel>(postEntity);
            var propertyTags = typeof(TPostViewModel).GetProperty("Tags");
            var propertyUserScore = typeof(TPostViewModel).GetProperty("UserScore");
            var propertyUserMiniViewModel = typeof(TPostViewModel).GetProperty("AuthorUserMiniViewModel");
            var propertyComments = typeof(TPostViewModel).GetProperty("Comments");

            if (propertyTags != null)
            {
                var tags = postEntity.Tags.Select(a => repositoryOfTag.Read(b => b.Id == a.TagId).Name);
                propertyTags.SetValue(postViewModel, tags);
            }

            if(propertyUserScore != null)
            {
                var postRating = repositoryOfPostRating.Read(a => a.UserProfileId == applicationUser.UserProfileId);
                if(postRating != null)
                {
                    propertyUserScore.SetValue(postViewModel, postRating.Score);
                }
            }

            if (propertyUserMiniViewModel != null)
            {
                UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUser);
                propertyUserMiniViewModel.SetValue(postViewModel, userMiniViewModel);
            }

            if (propertyComments != null)
            {
                var comentsViewModels = new List<CommentViewModel>();
                var currentApplicationUser = repositoryOfApplicationUser.Read(a => a.Id == currentApplicationUserId);

                foreach (var commentEntity in postEntity.Comments)
                {
                    var applicationUserForComment = repositoryOfUserProfile.Read(a => a.Id == commentEntity.UserProfileId, a => a.ApplicationUser).ApplicationUser;
                    UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserForComment);
                    var commentViewModel = mapper.Map<CommentEntity, CommentViewModel>(commentEntity);
                    commentViewModel.AuthorUserMiniViewModel = userMiniViewModel;

                    var commentLikeEntity = repositoryOfCommentLike.Read(a => a.CommentId == commentEntity.Id && a.UserProfileId == currentApplicationUser.UserProfileId);
                    commentViewModel.IsUserLiked = commentLikeEntity != null;

                    comentsViewModels.Add(commentViewModel);
                }
                propertyComments.SetValue(postViewModel, comentsViewModels);
            }

            return postViewModel;
        }

        public PostViewModel Update(PostCreateEditViewModel postCreateEditViewModel)
        {
            var postEntity = mapper.Map<PostCreateEditViewModel, PostEntity>(postCreateEditViewModel);
            repositoryOfPost.Update(postEntity);
            postEntity = repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId);
            var postViewModel = mapper.Map<PostEntity, PostViewModel>(postEntity);
            return postViewModel;
        }

        public void RatingPost(string applicationUserId, int postId, byte score) 
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var lastPostRating = repositoryOfPostRating.Read(a => a.PostId == postId && a.UserProfileId == applicationUser.UserProfileId);

            if (lastPostRating == null)
            {
                var postRating = new PostRatingEntity()
                {
                    PostId = postId,
                    UserProfileId = applicationUser.UserProfileId,
                    Score = score
                };
                repositoryOfPostRating.Create(postRating);
            }
            else
            {
                var updatePostRating = new PostRatingEntity()
                {
                    Id = lastPostRating.Id,
                    UserProfileId = lastPostRating.UserProfileId,
                    PostId = lastPostRating.PostId,
                    Score = score
                };
                repositoryOfPostRating.Update(updatePostRating);
            }
        }

        private void Delete<TPostViewModel>(TPostViewModel postViewModel)
        {
            var postEntity = mapper.Map<TPostViewModel, PostEntity>(postViewModel);
            if(postEntity.Images == null)
            {
                int id = Convert.ToInt32(typeof(TPostViewModel).GetProperty("PostId").GetValue(postViewModel));
                postEntity = repositoryOfPost.Read(a => a.Id == id, a => a.Images);
            }
            postEntity.Images.ToList().ForEach(a => File.Delete(a.Path));
            repositoryOfPost.Delete(postEntity);
        }
    }
}
