using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Implementation.Services
{
    public class ServiceOfPost
    {
        private readonly IMapper mapper;

        private readonly RepositoryOfPost repositoryOfPost;
        private readonly RepositoryOfPostRating repositoryOfPostRating;
        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
        private readonly RepositoryOfSection repositoryOfSection;
        private readonly RepositoryOfImage repositoryOfImage;

        private readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfAccount serviceOfAccount;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfTag serviceOfTag;

        public Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>[] Config;

        public ServiceOfPost(
            ApplicationDbContext context, 
            IMapper mapper,
            ServiceOfImage serviceOfImage,
            ServiceOfAccount serviceOfAccount,
            ServiceOfComment serviceOfComment,
            ServiceOfUser serviceOfUser,
            ServiceOfTag serviceOfTag
            )
        {
            this.mapper = mapper;

            repositoryOfPost = new RepositoryOfPost(context);
            repositoryOfPostRating = new RepositoryOfPostRating(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
            repositoryOfSection = new RepositoryOfSection(context);
            repositoryOfImage = new RepositoryOfImage(context);

            this.serviceOfImage = serviceOfImage;
            this.serviceOfAccount = serviceOfAccount;
            this.serviceOfComment = serviceOfComment;
            this.serviceOfUser = serviceOfUser;
            this.serviceOfTag = serviceOfTag;

            Config = new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>[]
            {
                new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(nameof(PostCompactViewModel), GetPostCompactViewModel),
                new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(nameof(PostUpdateViewModel), GetPostUpdateViewModel),
                new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(nameof(PostMiniViewModel), GetPostMiniViewModel),
                new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(nameof(PostViewModel), GetPostViewModel)
            };
        }

        public IEnumerable<PostCompactViewModel> Search(string propetry)
        {
            return repositoryOfPost
                .ReadMany(new Expression<Func<PostEntity, bool>>[] { a => a.Header.Contains(propetry) })
                .Select(a => GetPostCompactViewModel(a, null))
                .AsParallel()
                .ToList();
        }
        public async Task<PostUpdateViewModel> Create(string applicationUserIdCurrent, PostUpdateViewModel postCreateEditViewModel)
        {
            var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if (!await serviceOfUser.IsThereAccess(new[] { 2, 3 }, applicationUserCurrent, null, false))
            {
                return null;
            }
            PostEntity postEntity = repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId, a => a.Tags, a => a.UserProfile);
            if (postEntity != null)
            {
                return null;
            }
            postEntity = (postCreateEditViewModel == null)
                ? new PostEntity()
                : postEntity = mapper.Map<PostUpdateViewModel, PostEntity>(postCreateEditViewModel);
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            postEntity.UserProfileId = applicationUser.UserProfileId;
            if (postCreateEditViewModel != null && postCreateEditViewModel.Section != null)
            {
                var section = repositoryOfSection.Read(a => a.Name == postCreateEditViewModel.Section);
                postEntity.Section = section;
                postEntity.SectionId = section.Id;
            }
            postEntity = repositoryOfPost.Create(postEntity);
            if (postCreateEditViewModel.Images != null && postCreateEditViewModel.Images.Count() > 0)
            {
                postEntity.Images = postCreateEditViewModel.Images.Select(a =>
                {
                    return repositoryOfImage.Create(new ImageEntity()
                    {
                        Path = a /*serviceOfImage.RenameImage("Post", a, postEntity.Id.ToString())*/,
                        PostId = postEntity.Id
                    });
                })
                .ToList();
            }
            if (postCreateEditViewModel != null && postCreateEditViewModel.Tags != null)
            {
                postEntity.Tags = serviceOfTag.AddTagsPost(postCreateEditViewModel.Tags, postEntity.Id);
            }
            var newPostCreateEditViewModel = mapper.Map<PostEntity, PostUpdateViewModel>(postEntity);
            return newPostCreateEditViewModel;
        }
        public void Update(string applicationUserIdCurrent, PostUpdateViewModel postCreateEditViewModel, PostEntity lastPostEntity = null)
        {
            var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent, a => a.UserProfile);
            Update(applicationUserCurrent, postCreateEditViewModel, lastPostEntity);
        }
        public async Task Update(ApplicationUserEntity applicationUserCurrent, PostUpdateViewModel postCreateEditViewModel, PostEntity lastPostEntity = null)
        {
            lastPostEntity = (lastPostEntity == null) ? repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId, a => a.Tags, a => a.UserProfile) : lastPostEntity;
            var postEntity = mapper.Map<PostUpdateViewModel, PostEntity>(postCreateEditViewModel);
            if (!await serviceOfUser.IsThereAccess(new[] { 2, 3 }, applicationUserCurrent, lastPostEntity.UserProfile.ApplicationUserId, true))
            {
                return;
            }
            if (postCreateEditViewModel.Section != null)
            {
                var section = repositoryOfSection.Read(a => a.Name == postCreateEditViewModel.Section);
                postEntity.Section = section;
                postEntity.SectionId = section.Id;
            }
            postEntity.Tags = serviceOfTag.TagsPostUpdate(postCreateEditViewModel.Tags, lastPostEntity.Tags, postCreateEditViewModel.PostId);
            repositoryOfPost.Update(postEntity);
        }
        public async Task Delete(string applicationUserIdCurrent, int postId)
        {
            var post = repositoryOfPost.Read(a => a.Id == postId, a => a.UserProfile, a => a.Images);
            if(post == null)
            {
                return;
            }
            var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if (await serviceOfUser.IsThereAccess(new[] { 3 }, applicationUserCurrent, post.UserProfile.ApplicationUserId, true))
            {
                post.Images.ToList().ForEach(a => File.Delete(a.Path));
                repositoryOfPost.Delete(post);
            }
            return;
        }
        public IEnumerable<BasePostViewModel> Get(string type, string applicationUserIdCurrent, int? take, string where, string orderBy)
        {
            IEnumerable<PostEntity> postsEntities = repositoryOfPost.ReadMany(where, orderBy,
                    a => a.Tags,
                    a => a.Comments,
                    a => a.Section,
                    a => a.UserProfile);
            if (take != null)
            {
                postsEntities = postsEntities.Take(take.Value).ToList();
            }
            var postsViewModel = postsEntities
                .Select(a => GetViewModelWithProperty(type, a, applicationUserIdCurrent))
                .AsParallel()
                .ToList();
            return postsViewModel;
        }
        private PostUpdateViewModel GetPostUpdateViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostUpdateViewModel>(postEntity);
            postViewModel.Tags = serviceOfTag.GetTagsForPost(postEntity);
            postViewModel.BelongsToUser = (applicationUserCurrent == null) 
                ? false 
                : serviceOfUser.IsThereAccess(new[] { 3 }, applicationUserCurrent, postEntity.UserProfile.ApplicationUserId, true).Result;
            return postViewModel;
        }
        private PostMiniViewModel GetPostMiniViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostMiniViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser.Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.BelongsToUser = (applicationUserCurrent == null)
                ? false
                : serviceOfUser.IsThereAccess(new[] { 3 }, applicationUserCurrent, postEntity.UserProfile.ApplicationUserId, true).Result;
            return postViewModel;
        }
        private PostCompactViewModel GetPostCompactViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostCompactViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser.Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserPost);
            postViewModel.BelongsToUser = (applicationUserCurrent == null)
                ? false
                : serviceOfUser.IsThereAccess(new[] { 3 }, applicationUserCurrent, postEntity.UserProfile.ApplicationUserId, true).Result;
            return postViewModel;
        }
        private PostViewModel GetPostViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser
                .Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.Tags = serviceOfTag.GetTagsForPost(postEntity);
            postViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserPost);
            postViewModel.Comments = serviceOfComment.GetMany<CommentViewModel>(postEntity, applicationUserCurrent);
            postViewModel.CurrentUserId = (applicationUserCurrent == null) ? null : applicationUserCurrent.Id;
            postViewModel.BelongsToUser = (applicationUserCurrent == null)
                ? false
                : serviceOfUser.IsThereAccess(new[] { 3 }, applicationUserCurrent, postEntity.UserProfile.ApplicationUserId, true).Result;
            return postViewModel;
        }
        public BasePostViewModel GetViewModelWithProperty(string type, PostEntity postEntity, string applicationUserIdCurrent)
        {
            var applicationUser = (applicationUserIdCurrent == null) 
                ? null
                : repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent, a => a.UserProfile);
            return GetViewModelWithProperty(type, postEntity, applicationUser);
        }
        public BasePostViewModel GetViewModelWithProperty(string type, PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            return Config.Single(a => a.Item1 == type).Item2(postEntity, applicationUserCurrent) as BasePostViewModel;
        }
        public double RatingPost(string applicationUserId, int postId, byte score)
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var lastPostRating = repositoryOfPostRating.Read(a => a.PostId == postId && a.UserProfileId == applicationUser.UserProfileId);
            var postRatingEntity = new PostRatingEntity()
            {
                PostId = postId,
                UserProfileId = applicationUser.UserProfileId,
                Score = score
            };
            if (lastPostRating == null)
            {
                repositoryOfPostRating.Create(postRatingEntity);
            }
            else
            {
                postRatingEntity.Id = lastPostRating.Id;
                repositoryOfPostRating.Update(postRatingEntity);
            }
            var post = repositoryOfPost.Read(a => a.Id == postId);
            return (post.SumOfScore / (double)post.CountOfScore);
        }
        public string AddImage(string applicationUserCurrent, PostImage image) => serviceOfImage.LoadImage(applicationUserCurrent, image);
    }
}