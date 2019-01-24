using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Contracts.Models.ViewModels.Tag;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        private readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfTag serviceOfTag;

        Tuple<Type, Func<PostEntity, string, BasePostViewModel>>[] Config;

        public ServiceOfPost(
            ApplicationDbContext context, 
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUserEntity> userManager,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper
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
            serviceOfComment = new ServiceOfComment(context, roleManager, userManager, hostingEnvironment, mapper);
            serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            serviceOfTag = new ServiceOfTag(context, mapper);

            Config = new Tuple<Type, Func<PostEntity, string, BasePostViewModel>>[]
            {
                new Tuple<Type, Func<PostEntity, string, BasePostViewModel>>(typeof(PostCreateEditViewModel), GetPostCreateEditViewModel),
                new Tuple<Type, Func<PostEntity, string, BasePostViewModel>>(typeof(PostMiniViewModel), GetPostMiniViewModel),
                new Tuple<Type, Func<PostEntity, string, BasePostViewModel>>(typeof(PostViewModel), GetPostViewModel)
            };
        }


        public void Update(PostCreateEditViewModel postCreateEditViewModel)
        {
            var postEntity = mapper.Map<PostCreateEditViewModel, PostEntity>(postCreateEditViewModel);
            var lastPostEntity = repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId,
                a => a.Tags
            );

            if (postCreateEditViewModel.Section != null)
            {
                var section = repositoryOfSection.Read(a => a.Name == postCreateEditViewModel.Section);
                postEntity.Section = section;
                postEntity.SectionId = section.Id;
            }
            var postTagEntities = postCreateEditViewModel
                .Tags
                .Select(a =>
                {
                    var tag = mapper.Map<TagViewModel, TagEntity>(a);
                    if (tag.Id == 0)
                    {
                        tag = repositoryOfTag.Create(new TagEntity() { Name = a.Name });
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
        }
        
        public int Create(string applicationUserIdCurrent, PostCreateEditViewModel postCreateEditViewModel)
        {
            PostEntity postEntity;

            if(postCreateEditViewModel == null)
            {
                postEntity = new PostEntity();
            }
            else
            {
                postEntity = mapper.Map<PostCreateEditViewModel, PostEntity>(postCreateEditViewModel);
            }
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            postEntity.UserProfileId = applicationUser.UserProfileId;
            if(postCreateEditViewModel != null && postCreateEditViewModel.Section != null)
            {
                var section = repositoryOfSection.Read(a => a.Name == postCreateEditViewModel.Section);
                postEntity.Section = section;
                postEntity.SectionId = section.Id;
            }

            postEntity = repositoryOfPost.Create(postEntity);

            if(postCreateEditViewModel != null && postCreateEditViewModel.Tags != null)
            {
                postCreateEditViewModel
                    .Tags
                    .Select(a =>
                    {
                        var tag = mapper.Map<TagViewModel, TagEntity>(a);
                        if (tag.Id == 0)
                        {
                            tag = repositoryOfTag.Create(new TagEntity() { Name = a.Name });
                        }
                        return tag;
                    })
                    .Select(a => repositoryOfPostTag.Create(new PostTagEntity()
                    {
                        PostId = postEntity.Id,
                        TagId = a.Id,
                        Tag = a,
                    }));
            }
            return postEntity.Id;
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
            var potsViewModels = posts.Select(a => GetViewModelWithProperty<TPostViewModel>(a, applicationUserIdCurrent) as TPostViewModel).ToList();
            return potsViewModels;
        }

        public TPostViewModel Get<TPostViewModel>(string applicationUserIdCurrent, int postId) where TPostViewModel : class
        {
            var postEntity = repositoryOfPost.Read(a => a.Id == postId, 
                a => a.Tags, 
                a => a.Comments, 
                a => a.Section, 
                a => a.UserProfile);
            var postViewModel = GetViewModelWithProperty<TPostViewModel>(postEntity, applicationUserIdCurrent);
            return postViewModel as TPostViewModel;
        }
        public double RatingPost(string applicationUserId, int postId, byte score)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var lastPostRating = repositoryOfPostRating.Read(a => a.PostId == postId && a.UserProfileId == applicationUser.UserProfileId);
            if (lastPostRating == null)
            {
                var postRatingEntity = new PostRatingEntity();
                postRatingEntity.PostId = postId;
                postRatingEntity.UserProfileId = applicationUser.UserProfileId;
                postRatingEntity.Score = score;
                repositoryOfPostRating.Create(postRatingEntity);
            }
            else
            {
                lastPostRating.Score = score;
                repositoryOfPostRating.Update(lastPostRating);
            }
            var post = repositoryOfPost.Read(a => a.Id == postId);
            return (post.SumOfScore / (double)post.CountOfScore);
        }

        private PostCreateEditViewModel GetPostCreateEditViewModel(PostEntity postEntity, string applicationUserIdCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostCreateEditViewModel>(postEntity);
            return postViewModel;
        }
        private PostMiniViewModel GetPostMiniViewModel(PostEntity postEntity, string applicationUserIdCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostMiniViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser
                .Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.Score = GetRatin(postEntity, applicationUserPost.UserProfileId.Value);
            postViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserPost);
            return postViewModel;
        }
        private PostViewModel GetPostViewModel(PostEntity postEntity, string applicationUserIdCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser
                .Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.Tags = serviceOfTag.GetTagsForPost(postEntity);
            postViewModel.Score = GetRatin(postEntity, applicationUserPost.UserProfileId.Value);
            postViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserPost);
            postViewModel.Comments = serviceOfComment.Get<CommentViewModel>(postEntity.Id, applicationUserIdCurrent); ;
            return postViewModel;
        }

        private BasePostViewModel GetViewModelWithProperty<TPostViewModel>(PostEntity postEntity, string applicationUserIdCurrent) =>
            Config.Single(a => a.Item1 == typeof(TPostViewModel)).Item2(postEntity, applicationUserIdCurrent);

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

        private int GetRatin(PostEntity postEntity, int UserProfileId)
        {
            var postRating = repositoryOfPostRating.Read(a => a.PostId == postEntity.Id && a.UserProfileId == UserProfileId);
            if (postRating != null)
            {
                return postRating.Score;
            }
            return -1;
        }
    }
}