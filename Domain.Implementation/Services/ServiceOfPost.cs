using AutoMapper;
using Data.Contracts;
using Data.Contracts.Models.Entities;
using Domain.Contracts.Models;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using Domain.Contracts.Models.ViewModels.Tag;
using Search.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Implementation.Services
{
    public class ServiceOfPost
    {
        private readonly IMapper mapper;
        private readonly IRepository<PostEntity> repositoryOfPost;
        private readonly IRepository<PostRatingEntity> repositoryOfPostRating;
        private readonly IRepository<ApplicationUserEntity> repositoryOfApplicationUser;
        private readonly IRepository<SectionEntity> repositoryOfSection;
        private readonly IRepository<ImageEntity> repositoryOfImage;
        private readonly ServiceOfComment serviceOfComment;
        private readonly ServiceOfAccount serviceOfAccount;
        private readonly ServiceOfImage serviceOfImage;
        private readonly ServiceOfUser serviceOfUser;
        private readonly ServiceOfTag serviceOfTag;
        private readonly ServiceOfSearch serviceOfSearch;
        public Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>[] Config;

        public ServiceOfPost(
            IMapper mapper,
            ServiceOfImage serviceOfImage,
            ServiceOfAccount serviceOfAccount,
            ServiceOfComment serviceOfComment,
            ServiceOfUser serviceOfUser,
            ServiceOfTag serviceOfTag,
            ServiceOfSearch serviceOfSearch,
            IRepository<PostEntity> repositoryOfPost,
            IRepository<PostRatingEntity> repositoryOfPostRating,
            IRepository<ApplicationUserEntity> repositoryOfApplicationUser,
            IRepository<SectionEntity> repositoryOfSection,
            IRepository<ImageEntity> repositoryOfImage
            )
        {
            this.mapper = mapper;
            this.repositoryOfPost = repositoryOfPost;
            this.repositoryOfPostRating = repositoryOfPostRating;
            this.repositoryOfApplicationUser = repositoryOfApplicationUser;
            this.repositoryOfSection = repositoryOfSection;
            this.repositoryOfImage = repositoryOfImage;
            this.serviceOfImage = serviceOfImage;
            this.serviceOfAccount = serviceOfAccount;
            this.serviceOfComment = serviceOfComment;
            this.serviceOfUser = serviceOfUser;
            this.serviceOfTag = serviceOfTag;
            this.serviceOfSearch = serviceOfSearch;

            Config = new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>[]
            {
                new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(nameof(PostCompactViewModel), GetPostCompactViewModel),
                new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(nameof(PostUpdateViewModel), GetPostUpdateViewModel),
                new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(nameof(PostMiniViewModel), GetPostMiniViewModel),
                new Tuple<string, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(nameof(PostViewModel), GetPostViewModel)
            };
        }

        public IEnumerable<PostCompactViewModel> Search(string applicationUserIdCurrent, string propetry, int? skip, int? take)
        {
            var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            var ids = serviceOfSearch.SearchPosts(propetry, (skip != null) ? skip.Value : 0, (take!=null) ? take.Value : 0);
            var result = ids
                .Select(b => repositoryOfPost.Read(a => a.Id == b,
                    a => a.Tags,
                    a => a.Comments,
                    a => a.Section,
                    a => a.UserProfile))
                .Select(a => GetViewModelWithProperty("PostCompactViewModel", a, applicationUserCurrent) as PostCompactViewModel)
                .ToList();
            return result;
        }
        private async Task<PostEntity> Create(string applicationUserIdCurrent, bool flag)
        {
            var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            if (!await serviceOfUser.IsThereAccess(new[] { 2, 3 }, applicationUserCurrent, null, false))
            {
                return null;
            }
            var newPost = repositoryOfPost.Create(new PostEntity()
            {
                Header = "New Post",
                Content = "# Content",
                UserProfileId = applicationUserCurrent.UserProfileId
            });
            return newPost;
        }
        public async Task<PostUpdateViewModel> Create(string applicationUserIdCurrent)
        {
            var newPost = await Create(applicationUserIdCurrent, false);
            var newPostViewModel = mapper.Map<PostEntity, PostUpdateViewModel>(newPost);
            newPostViewModel.Tags = new List<TagViewModel>();
            newPostViewModel.Images = new List<string>();
            return newPostViewModel;
        }
        public async Task<PostUpdateViewModel> Create(string applicationUserIdCurrent, PostUpdateViewModel postCreateEditViewModel)
        {
            var newPost = await Create(applicationUserIdCurrent, false);
            postCreateEditViewModel.PostId = newPost.Id;
            await Update(applicationUserIdCurrent, postCreateEditViewModel, newPost);
            var post = repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId, 
                a => a.Tags,
                a => a.Comments,
                a => a.Section,
                a => a.UserProfile);
            return GetViewModelWithProperty("PostUpdateViewModel", post, applicationUserIdCurrent) as PostUpdateViewModel;
        }
        public async Task Update(string applicationUserIdCurrent, PostUpdateViewModel postCreateEditViewModel, PostEntity lastPostEntity = null)
        {
            var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent, a => a.UserProfile);
            await Update(applicationUserCurrent, postCreateEditViewModel, lastPostEntity);
        }
        public async Task Update(ApplicationUserEntity applicationUserCurrent, PostUpdateViewModel postCreateEditViewModel, PostEntity lastPostEntity = null)
        {
            lastPostEntity = (lastPostEntity == null) ? repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId, a => a.Tags, a => a.UserProfile, a => a.Images) : lastPostEntity;
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
            if (postEntity.IsFinished == false && lastPostEntity.IsFinished == true)
            {
                postEntity.IsFinished = true;
            }
            postEntity.CountOfScore = lastPostEntity.CountOfScore;
            postEntity.SumOfScore = lastPostEntity.SumOfScore;
            postEntity.Created = lastPostEntity.Created;
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
                post.Images.ToList().ForEach(a => serviceOfImage.Delete(a.Path));
                repositoryOfPost.Delete(post);
            }
        }
        public IEnumerable<BasePostViewModel> Get(string type, string applicationUserIdCurrent, int? skip, int? take, string where, string orderBy)
        {
            IEnumerable<PostEntity> postsEntities = repositoryOfPost.ReadMany(where, orderBy,
                    a => a.Tags,
                    a => a.Comments,
                    a => a.Section,
                    a => a.UserProfile);
            postsEntities = skip.HasValue ? postsEntities.Skip(skip.Value) : postsEntities;
            postsEntities = take.HasValue ? postsEntities.Take(take.Value) : postsEntities;
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
            postViewModel.Images = repositoryOfImage.ReadMany(new Expression<Func<ImageEntity, bool>>[] { a => a.PostId == postEntity.Id }).Select(a => a.Path).ToList();
            postViewModel.BelongsToUser = (applicationUserCurrent == null) 
                ? false 
                : applicationUserCurrent.UserProfileId == postEntity.UserProfileId;
            return postViewModel;
        }
        private PostMiniViewModel GetPostMiniViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostMiniViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser.Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.BelongsToUser = (applicationUserCurrent == null)
                ? false
                : applicationUserCurrent.UserProfileId == postEntity.UserProfileId;
            return postViewModel;
        }
        private PostCompactViewModel GetPostCompactViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostCompactViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser.Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserPost);
            postViewModel.BelongsToUser = (applicationUserCurrent == null)
                ? false
                : applicationUserCurrent.UserProfileId == postEntity.UserProfileId;
            postViewModel.FirstImage = (postEntity.Images == null)
                ? repositoryOfImage.ReadMany(new Expression<Func<ImageEntity, bool>>[] { a => a.PostId == postEntity.Id }).FirstOrDefault()?.Path
                : postEntity.Images.FirstOrDefault().Path;
            return postViewModel;
        }
        private PostViewModel GetPostViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser
                .Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.Tags = serviceOfTag.GetTagsForPost(postEntity);
            postViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserPost);
            postViewModel.Comments = serviceOfComment.GetMany(nameof(CommentViewModel), postEntity.Id, null, null, applicationUserCurrent) as List<CommentViewModel>;
            postViewModel.CurrentUserId = (applicationUserCurrent == null) ? null : applicationUserCurrent.Id;
            postViewModel.BelongsToUser = (applicationUserCurrent == null)
                ? false
                : applicationUserCurrent.UserProfileId == postEntity.UserProfileId;
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
        public string AddImage(string host, string applicationUserCurrent, PostImage image)
        {
            var result = serviceOfImage.LoadImage(host, applicationUserCurrent, image);
            repositoryOfImage.Create(new ImageEntity() { Path = result, PostId = image.PostId });
            return result;
        }
    }
}