using Data.Contracts.Models.Entities;
using Search.Implementation;
using System.Linq;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfUserProfile : DefaultRepository<UserProfileEntity>
    {
        private readonly ServiceOfSearch serviceOfSearch;
        public RepositoryOfUserProfile(ApplicationDbContext context, ServiceOfSearch serviceOfSearch) : base(context)
        {
            this.serviceOfSearch = serviceOfSearch;
        }

        public override void Delete(UserProfileEntity entity)
        {
            RepositoryOfPost repositoryOfPost = new RepositoryOfPost(context, serviceOfSearch);
            var posts = entity.Posts;
            if(posts == null)
            {
                posts = repositoryOfPost.ReadMany(new System.Linq.Expressions.Expression<System.Func<PostEntity, bool>>[] { a => a.UserProfileId == entity.Id });
            }
            posts = posts.ToList();
            if (posts != null)
            {
                foreach (var item in posts)
                {
                    repositoryOfPost.Delete(item);
                }
            }

            base.Delete(entity);
        }
    }
}
