using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfPostRating : DefaultRepository<PostRatingEntity>
    {
        public RepositoryOfPostRating(ApplicationDbContext context) : base(context)
        { }

        public override PostRatingEntity Create(PostRatingEntity entity)
        {
            RepositoryOfPost repositoryOfPost = new RepositoryOfPost(context);

            var Post = entity.Post;
            if (Post == null)
            {
                Post = repositoryOfPost.Read(a => a.Id == entity.PostId);
            }

            Post.SumOfScore += entity.Score;
            Post.CountOfScore++;
            repositoryOfPost.Update(Post);

            return base.Create(entity);
        }

        public override void Delete(PostRatingEntity entity)
        {
            RepositoryOfPost repositoryOfPost = new RepositoryOfPost(context);
            
            var Post = entity.Post;
            if (Post == null)
            {
                Post = repositoryOfPost.Read(a => a.Id == entity.PostId);
            }

            Post.SumOfScore -= entity.Score;
            Post.CountOfScore--;
            repositoryOfPost.Update(Post);
            
            base.Delete(entity);
        }

        public override void Update(PostRatingEntity entity)
        {
            RepositoryOfPost repositoryOfPost = new RepositoryOfPost(context);
            RepositoryOfPostRating repositoryOfPostRating = new RepositoryOfPostRating(context);

            var lastPostRating = repositoryOfPostRating.Read(a => a.Id == entity.Id);
            var Post = entity.Post;
            if (Post == null)
            {
                Post = repositoryOfPost.Read(a => a.Id == entity.PostId);
            }

            Post.SumOfScore -= lastPostRating.Score;
            Post.SumOfScore += entity.Score;
            repositoryOfPost.Update(Post);

            base.Update(entity);
        }
    }
}
