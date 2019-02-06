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

        public ServiceOfComment serviceOfComment { get; set; }
        public ServiceOfAccount serviceOfAccount { get; set; }
        public ServiceOfImage serviceOfImage { get; set; }
        public ServiceOfUser serviceOfUser { get; set; }
        public ServiceOfTag serviceOfTag { get; set; }

        Tuple<Type, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>[] Config;

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

            //serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            //serviceOfComment = new ServiceOfComment(context, roleManager, userManager, hostingEnvironment, mapper);
            //serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            //serviceOfTag = new ServiceOfTag(context, mapper);
            //serviceOfAccount = new ServiceOfAccount(context, userManager, roleManager, hostingEnvironment, mapper);

            Config = new Tuple<Type, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>[]
            {
                new Tuple<Type, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(typeof(PostCompactViewModel), GetPostCompactViewModel),
                new Tuple<Type, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(typeof(PostUpdateViewModel), GetPostUpdateViewModel),
                new Tuple<Type, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(typeof(PostMiniViewModel), GetPostMiniViewModel),
                new Tuple<Type, Func<PostEntity, ApplicationUserEntity, BasePostViewModel>>(typeof(PostViewModel), GetPostViewModel)
            };
        }

        public void Update(PostUpdateViewModel postCreateEditViewModel, PostEntity lastPostEntity = null)
        {
            var postEntity = mapper.Map<PostUpdateViewModel, PostEntity>(postCreateEditViewModel);
            lastPostEntity = (lastPostEntity == null) ? repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId, a => a.Tags) : lastPostEntity;
            if (postCreateEditViewModel.Section != null)
            {
                var section = repositoryOfSection.Read(a => a.Name == postCreateEditViewModel.Section);
                postEntity.Section = section;
                postEntity.SectionId = section.Id;
            }
            postEntity.Tags = serviceOfTag.TagsPostUpdate(postCreateEditViewModel.Tags, lastPostEntity.Tags, postCreateEditViewModel.PostId);
            repositoryOfPost.Update(postEntity);
        }
        public async Task<PostUpdateViewModel> Create(string applicationUserIdCurrent, PostUpdateViewModel postCreateEditViewModel)
        {
            PostEntity postEntity = repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId, a => a.Tags, a => a.UserProfile);
            if(postEntity != null)
            {
                var applicationUserCurrent = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
                if (await serviceOfAccount.IsThereAccess(applicationUserCurrent, postEntity.UserProfile.ApplicationUserId))
                {
                    Update(postCreateEditViewModel, postEntity);
                    return postCreateEditViewModel;
                }
                return null;
            }

            postEntity = (postCreateEditViewModel == null)
                ? new PostEntity()
                : postEntity = mapper.Map<PostUpdateViewModel, PostEntity>(postCreateEditViewModel);
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
            postEntity.UserProfileId = applicationUser.UserProfileId;
            if(postCreateEditViewModel != null && postCreateEditViewModel.Section != null)
            {
                var section = repositoryOfSection.Read(a => a.Name == postCreateEditViewModel.Section);
                postEntity.Section = section;
                postEntity.SectionId = section.Id;
            }
            postEntity = repositoryOfPost.Create(postEntity);
            if(postCreateEditViewModel.Images != null && postCreateEditViewModel.Images.Count() > 0)
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
            if(postCreateEditViewModel != null && postCreateEditViewModel.Tags != null)
            {
                postEntity.Tags = serviceOfTag.AddTagsPost(postCreateEditViewModel.Tags, postCreateEditViewModel.PostId);
            }
            var newPostCreateEditViewModel = mapper.Map<PostEntity, PostUpdateViewModel>(postEntity);
            return newPostCreateEditViewModel;
        }
        public async Task Delete(string applicationUserIdCurrent, int postId)
        {
            var post = repositoryOfPost.Read(a => a.Id == postId, a => a.UserProfile);
            if(post == null)
            {
                return;
            }
            if(applicationUserIdCurrent != post.UserProfile.ApplicationUserId)
            {
                var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent);
                var role = await serviceOfUser.GetUserRole(applicationUser);
                if (role.Item1 != "admin")
                {
                    return;
                }
            }
            repositoryOfPost.Delete(post);
        }

        public IEnumerable<TPostViewModel> Get<TPostViewModel>(
            string applicationUserIdCurrent, 
            int? take, 
            Expression<Func<PostEntity, object>> orderBy = null, 
            params Expression<Func<PostEntity, bool>>[] whereProperties)
            where TPostViewModel : BasePostViewModel
        {
            IEnumerable<PostEntity> postsEntities = repositoryOfPost.ReadMany(whereProperties,
                    a => a.Tags,
                    a => a.Comments,
                    a => a.Section,
                    a => a.UserProfile);
            if (orderBy != null)
            {
                postsEntities = postsEntities.OrderBy(orderBy.Compile());
            }
            if (take != null)
            {
                postsEntities = postsEntities.Take(take.Value);
            }
            var postsViewModel = postsEntities
                .Select(a => GetViewModelWithProperty<TPostViewModel>(a, applicationUserIdCurrent))
                .AsParallel()
                .ToList();
            return postsViewModel;
        }

        public TPostViewModel Get<TPostViewModel>(string applicationUserIdCurrent, int postId) where TPostViewModel : BasePostViewModel
        {
            var postEntity = repositoryOfPost.Read(a => a.Id == postId, 
                a => a.Tags, 
                a => a.Comments, 
                a => a.Section, 
                a => a.UserProfile);
            var postViewModel = GetViewModelWithProperty<TPostViewModel>(postEntity, applicationUserIdCurrent);
            return postViewModel;
        }
        
        private PostUpdateViewModel GetPostUpdateViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostUpdateViewModel>(postEntity);
            postViewModel.Tags = serviceOfTag.GetTagsForPost(postEntity);
            postViewModel.BelongsToUser = (applicationUserCurrent == null) 
                ? false 
                : serviceOfAccount.IsThereAccess(applicationUserCurrent, postEntity.UserProfile.ApplicationUserId).Result;
            return postViewModel;
        }
        private PostMiniViewModel GetPostMiniViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostMiniViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser.Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.BelongsToUser = (applicationUserCurrent == null)
                ? false
                : serviceOfAccount.IsThereAccess(applicationUserCurrent, postEntity.UserProfile.ApplicationUserId).Result;
            return postViewModel;
        }
        private PostCompactViewModel GetPostCompactViewModel(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostCompactViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser.Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.AuthorUserMiniViewModel = serviceOfUser.GetUserMiniViewModel(applicationUserPost);
            postViewModel.BelongsToUser = (applicationUserCurrent == null)
                ? false
                : serviceOfAccount.IsThereAccess(applicationUserCurrent, postEntity.UserProfile.ApplicationUserId).Result;
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
                : serviceOfAccount.IsThereAccess(applicationUserCurrent, postEntity.UserProfile.ApplicationUserId).Result;
            return postViewModel;
        }

        public TPostViewModel GetViewModelWithProperty<TPostViewModel>(PostEntity postEntity, string applicationUserIdCurrent)
            where TPostViewModel : BasePostViewModel
        {
            var applicationUser = (applicationUserIdCurrent == null) 
                ? null
                : repositoryOfApplicationUser.Read(a => a.Id == applicationUserIdCurrent, a => a.UserProfile);
            return GetViewModelWithProperty<TPostViewModel>(postEntity, applicationUser);
        }
        public TPostViewModel GetViewModelWithProperty<TPostViewModel>(PostEntity postEntity, ApplicationUserEntity applicationUserCurrent)
            where TPostViewModel : BasePostViewModel
        {
            return Config.Single(a => a.Item1 == typeof(TPostViewModel)).Item2(postEntity, applicationUserCurrent) as TPostViewModel;
        }

        private void Delete<TPostViewModel>(TPostViewModel postViewModel) where TPostViewModel: BaseCommentViewModel
        {
            var postEntity = mapper.Map<TPostViewModel, PostEntity>(postViewModel);
            if(postEntity.Images == null)
            {
                postEntity = repositoryOfPost.Read(a => a.Id == postEntity.Id, a => a.Images);
            }
            postEntity.Images.ToList().ForEach(a => File.Delete(a.Path));
            repositoryOfPost.Delete(postEntity);
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