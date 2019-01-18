using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Contracts.Models.ViewModels.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfUser serviceOfUser;

        public ServiceOfPost(
            ApplicationDbContext context, 
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUserEntity> userManager,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment
            )
        {
            this.mapper = mapper;

            repositoryOfPost = new RepositoryOfPost(context);
            repositoryOfPostRating = new RepositoryOfPostRating(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfTag = new RepositoryOfTag(context);
            repositoryOfPostTag = new RepositoryOfPostTag(context);
            repositoryOfSection = new RepositoryOfSection(context);

            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfComment = new ServiceOfComment(context, mapper);
            serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
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

        public PostViewModel Update(PostCreateEditViewModel postCreateEditViewModel, bool isFinishe = false)
        {
            var postEntity = mapper.Map<PostCreateEditViewModel, PostEntity>(postCreateEditViewModel);
            var lastPostEntity = repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId,
                a => a.Tags
            );

            var section = repositoryOfSection.Read(a => a.Id == Int32.Parse(postCreateEditViewModel.Section));
            postEntity.Section = section;
            postEntity.SectionId = section.Id;

            var postTagEntities = postCreateEditViewModel
                .Tags
                .Split(" ")
                .Distinct()
                .Select(a => {
                    var tag = repositoryOfTag.Read(b => b.Name == a);
                    if (tag == null)
                    {
                        tag = repositoryOfTag.Create(new TagEntity() { Name = a });
                    }
                    return tag;
                });
            lastPostEntity
                .Tags
                .Select(a => a.TagId.Value)
                .Except(postTagEntities.Select(a => a.Id))
                .Select(a => lastPostEntity.Tags.Single(b => b.Id == a))
                .ToList()
                .ForEach(a => repositoryOfPostTag.Delete(a));
            var newPostTags = postTagEntities
                .Select(a => a.Id)
                .Except(lastPostEntity.Tags.Select(a => a.TagId.Value))
                .Select(a => postTagEntities.Single(b => b.Id == a))
                .Select(a => repositoryOfPostTag.Create(new PostTagEntity()
                {
                    PostId = postEntity.Id,
                    TagId = a.Id,
                    Tag = a,
                }));
            postEntity.Tags = newPostTags;
            repositoryOfPost.Update(postEntity);
            if(isFinishe)
            {
                var postViewModel = mapper.Map<PostEntity, PostViewModel>(postEntity);
                return postViewModel;
            }
            return null;
        }

        public PostViewModel CreateFinished(PostCreateEditViewModel postCreateEditViewModel, bool isReturnViewModel = false)
        {
            var postEntity = mapper.Map<PostCreateEditViewModel, PostEntity>(postCreateEditViewModel);
            
            var section = repositoryOfSection.Read(a => a.Id == Int32.Parse(postCreateEditViewModel.Section));
            postEntity.Section = section;
            postEntity.SectionId = section.Id;
            
            var postTagEntities = postCreateEditViewModel
                .Tags
                .Split(" ")
                .Distinct()
                .Select(a => {
                    var tag = repositoryOfTag.Read(b => b.Name == a);
                    if (tag == null)
                    {
                        tag = repositoryOfTag.Create(new TagEntity() { Name = a });
                    }
                    return tag;
                })
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
        
        public IEnumerable<TPostViewModel> Get<TPostViewModel>(string applicationUserIdCurrent, bool postIsFinished = true) 
            where TPostViewModel : class
        {
            var posts = repositoryOfPost.ReadMany(null, 
                    a => a.Tags, 
                    a => a.Comments, 
                    a => a.Section, 
                    a => a.UserProfile);
            posts = posts.Where(a => a.IsFinished == postIsFinished);
            var potsViewModels = posts.Select(a => GetViewModelWithProperty<TPostViewModel>(a, applicationUserIdCurrent)).ToList();
            return potsViewModels;
        }

        public async Task<TPostViewModel> Get<TPostViewModel>(string applicationUserIdCurrent, int postId) where TPostViewModel : class
        {
            var postEntity = repositoryOfPost.Read(a => a.Id == postId, 
                a => a.Tags, 
                a => a.Comments, 
                a => a.Section, 
                a => a.UserProfile);
            var postViewModel = GetViewModelWithProperty<TPostViewModel>(postEntity, applicationUserIdCurrent);
            return postViewModel;
        }

        private TPostViewModel GetViewModelWithProperty<TPostViewModel>(PostEntity postEntity, string applicationUserIdCurrent)
        {
            TPostViewModel postViewModel = mapper.Map<PostEntity, TPostViewModel>(postEntity);

            var propertyTags = typeof(TPostViewModel).GetProperty("Tags");
            var propertyUserScore = typeof(TPostViewModel).GetProperty("UserScore");
            var propertyUserMiniViewModel = typeof(TPostViewModel).GetProperty("AuthorUserMiniViewModel");
            var propertyComments = typeof(TPostViewModel).GetProperty("Comments");

            ApplicationUserEntity applicationUserPost = repositoryOfApplicationUser
                .Read(a => a.UserProfileId == postEntity.UserProfileId);

            if (propertyTags != null)
            {
                var tags = postEntity.Tags.Select(a => repositoryOfTag.Read(b => b.Id == a.TagId).Name);
                propertyTags.SetValue(postViewModel, tags);
            }
            if (propertyUserScore != null)
            {
                var postRating = repositoryOfPostRating.Read(a => a.PostId == postEntity.Id && a.UserProfileId == applicationUserPost.UserProfileId);
                if (postRating != null)
                {
                    propertyUserScore.SetValue(postViewModel, postRating.Score);
                }
            }
            if (propertyUserMiniViewModel != null)
            {
                UserMiniViewModel userMiniViewModel = mapper.Map<ApplicationUserEntity, UserMiniViewModel>(applicationUserPost);
                var role = serviceOfUser.GetUserRole(applicationUserPost).Result;
                userMiniViewModel.Role = role.Item1;
                userMiniViewModel.RoleColor = role.Item2;
                propertyUserMiniViewModel.SetValue(postViewModel, userMiniViewModel);
            }
            if (propertyComments != null)
            {
                var comentsViewModels = serviceOfComment.Get<CommentViewModel>(postEntity.Id, applicationUserIdCurrent);
                propertyComments.SetValue(postViewModel, comentsViewModels);
            }
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
