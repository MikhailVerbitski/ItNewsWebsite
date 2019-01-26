using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Comment;
using Domain.Contracts.Models.ViewModels.Post;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Domain.Implementation.Services
{
    public class ServiceOfPost
    {
        private readonly IMapper mapper;

        private readonly RepositoryOfPost repositoryOfPost;
        private readonly RepositoryOfPostRating repositoryOfPostRating;
        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;
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
            repositoryOfSection = new RepositoryOfSection(context);

            serviceOfImage = new ServiceOfImage(context, hostingEnvironment);
            serviceOfComment = new ServiceOfComment(context, roleManager, userManager, hostingEnvironment, mapper);
            serviceOfUser = new ServiceOfUser(context, roleManager, userManager, mapper, hostingEnvironment);
            serviceOfTag = new ServiceOfTag(context, mapper);

            Config = new Tuple<Type, Func<PostEntity, string, BasePostViewModel>>[]
            {
                new Tuple<Type, Func<PostEntity, string, BasePostViewModel>>(typeof(PostCreateEditViewModel), GetPostCreateEditViewModel),
                new Tuple<Type, Func<PostEntity, string, BasePostViewModel>>(typeof(PostCompactViewModel), GetPostCompactViewModel),
                new Tuple<Type, Func<PostEntity, string, BasePostViewModel>>(typeof(PostMiniViewModel), GetPostMiniViewModel),
                new Tuple<Type, Func<PostEntity, string, BasePostViewModel>>(typeof(PostViewModel), GetPostViewModel)
            };
        }

        public void Update(PostCreateEditViewModel postCreateEditViewModel, IFormFile[] images)
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
            postEntity.Tags = serviceOfTag.TagsPostUpdate(postCreateEditViewModel.Tags, lastPostEntity.Tags, postCreateEditViewModel.PostId); ;
            repositoryOfPost.Update(postEntity);
            serviceOfImage.AddImage(postCreateEditViewModel.PostId, images);
        }
        
        public PostEntity Create(string applicationUserIdCurrent, PostCreateEditViewModel postCreateEditViewModel)
        {
            PostEntity postEntity = (postCreateEditViewModel == null)
                ? new PostEntity()
                : postEntity = mapper.Map<PostCreateEditViewModel, PostEntity>(postCreateEditViewModel);
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
                postEntity.Tags = serviceOfTag.AddTagsPost(postCreateEditViewModel.Tags, postCreateEditViewModel.PostId);
            }
            return postEntity;
        }

        public IEnumerable<TPostViewModel> Get<TPostViewModel>(string applicationUserIdCurrent, params Expression<Func<PostEntity, bool>>[] whereProperties)
            where TPostViewModel : BasePostViewModel => repositoryOfPost.ReadMany(whereProperties,
                    a => a.Tags,
                    a => a.Comments,
                    a => a.Section,
                    a => a.UserProfile)
                    .Select(a => GetViewModelWithProperty<TPostViewModel>(a, applicationUserIdCurrent))
                    .AsParallel()
                    .ToList();

        public BasePostViewModel Get<TPostViewModel>(string applicationUserIdCurrent, int postId) where TPostViewModel : BasePostViewModel
        {
            var postEntity = repositoryOfPost.Read(a => a.Id == postId, 
                a => a.Tags, 
                a => a.Comments, 
                a => a.Section, 
                a => a.UserProfile);
            var postViewModel = GetViewModelWithProperty<TPostViewModel>(postEntity, applicationUserIdCurrent);
            return postViewModel;
        }
        
        private PostCreateEditViewModel GetPostCreateEditViewModel(PostEntity postEntity, string applicationUserIdCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostCreateEditViewModel>(postEntity);
            return postViewModel;
        }
        private PostMiniViewModel GetPostMiniViewModel(PostEntity postEntity, string applicationUserIdCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostMiniViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser.Read(a => a.UserProfileId == postEntity.UserProfileId);
            postViewModel.Score = GetRatin(postEntity, applicationUserPost.UserProfileId.Value);
            return postViewModel;
        }
        private PostCompactViewModel GetPostCompactViewModel(PostEntity postEntity, string applicationUserIdCurrent)
        {
            var postViewModel = mapper.Map<PostEntity, PostCompactViewModel>(postEntity);
            var applicationUserPost = repositoryOfApplicationUser.Read(a => a.UserProfileId == postEntity.UserProfileId);
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
            postViewModel.Comments = serviceOfComment.Get<CommentViewModel>(postEntity.Id, applicationUserIdCurrent);
            return postViewModel;
        }

        private TPostViewModel GetViewModelWithProperty<TPostViewModel>(PostEntity postEntity, string applicationUserIdCurrent)
            where TPostViewModel : BasePostViewModel 
            => Config.Single(a => a.Item1 == typeof(TPostViewModel)).Item2(postEntity, applicationUserIdCurrent) as TPostViewModel;

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
        private int GetRatin(PostEntity postEntity, int UserProfileId)
        {
            var postRating = repositoryOfPostRating.Read(a => a.PostId == postEntity.Id && a.UserProfileId == UserProfileId);
            return postRating != null ? postRating.Score : -1;
        }
    }
}