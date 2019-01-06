using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Post;

namespace Domain.Implementation.Services
{
    public class ServiceOfPost
    {
        private readonly IMapper mapper;

        private readonly RepositoryOfPost repositoryOfPost;
        private readonly RepositoryOfPostRating repositoryOfPostRating;
        private readonly RepositoryOfApplicationUser repositoryOfApplicationUser;

        public ServiceOfPost(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;

            repositoryOfPost = new RepositoryOfPost(context);
            repositoryOfPostRating = new RepositoryOfPostRating(context);
            repositoryOfApplicationUser = new RepositoryOfApplicationUser(context);
        }

        public PostViewModel Create(string applicationUserId, PostCreateEditViewModel postCreateEditViewModel)
        {
            var postEntity = mapper.Map<PostCreateEditViewModel, PostEntity>(postCreateEditViewModel);
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            postEntity.UserProfileId = applicationUser.UserProfileId;

            //tags manipulation

            postEntity = repositoryOfPost.Create(postEntity);
            var postViewModel = mapper.Map<PostEntity, PostViewModel>(postEntity);
            return postViewModel;
        }

        //public IEnumerable<TPostViewModel> Get<TPostViewModel>(int PostId) where TPostViewModel : class
        //{
        //    var post = repositoryOfPost.Read(a => a.Id == PostId, a => a.Comments);
        //    var commentEntities = post.Comments.ToList();
        //    var commentViewModel = mapper.Map<IEnumerable<CommentEntity>, IEnumerable<TPostViewModel>>(commentEntities);
        //    return commentViewModel.ToList();
        //}

        public PostViewModel Update(PostCreateEditViewModel postCreateEditViewModel)
        {
            var postEntity = mapper.Map<PostCreateEditViewModel, PostEntity>(postCreateEditViewModel);
            repositoryOfPost.Update(postEntity);
            postEntity = repositoryOfPost.Read(a => a.Id == postCreateEditViewModel.PostId);
            var postViewModel = mapper.Map<PostEntity, PostViewModel>(postEntity);
            return postViewModel;
        }

        private TPostViewModel RatingPost<TPostViewModel>(string applicationUserId, int postId, byte score) 
            where TPostViewModel : class
        {
            var applicationUser = repositoryOfApplicationUser.Read(a => a.Id == applicationUserId);
            var postRating = new PostRatingEntity()
            {
                PostId = postId,
                UserProfileId = applicationUser.UserProfileId,
                Score = score
            };
            var postRatingEntity = repositoryOfPostRating.Create(postRating);

            if(typeof(TPostViewModel) == null)
            {
                return null;
            }

            var postEntity = repositoryOfPost.Read(a => a.Id == postRatingEntity.PostId);
            var postViewModel = mapper.Map<PostEntity, TPostViewModel>(postEntity);
            return postViewModel;
        }

        private void Delete<TPostViewModel>(TPostViewModel postViewModel)
        {
            var postEntity = mapper.Map<TPostViewModel, PostEntity>(postViewModel);
            repositoryOfPost.Delete(postEntity);
        }
    }
}
