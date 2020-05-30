using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Contracts;
using Data.Contracts.Models.Entities;
using Search.Implementation;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfPost : DefaultRepository<PostEntity>
    {
        private readonly ServiceOfSearch serviceOfSearch;
        public RepositoryOfPost(ApplicationDbContext context, ServiceOfSearch serviceOfSearch) : base(context)
        {
            this.serviceOfSearch = serviceOfSearch;
        }

        public override PostEntity Create(PostEntity entity)
        {
            IRepository<SectionEntity> repositoryOfSection = new RepositoryOfSection(context);

            var section = entity.Section;
            if(section == null)
            {
                section = repositoryOfSection.Read(a => a.Id == entity.SectionId);
            }
            if(section != null)
            {
                section.CountOfUsage++;
                repositoryOfSection.Update(section);
            }
            if(entity.IsFinished)
            {
                entity.Created = DateTime.Now;
            }
            entity = base.Create(entity);
            serviceOfSearch.Create<PostEntity>(entity);
            return entity;
        }

        public override PostEntity Update(PostEntity entity, params Expression<Func<PostEntity, object>>[] properties)
        {
            var lastPost = this.Read(a => a.Id == entity.Id, a => a.Section);
            IRepository<SectionEntity> repositoryOfSection = new RepositoryOfSection(context);
            
            if(entity.Section == null)
            {
                entity.Section = repositoryOfSection.Read(a => a.Id == entity.SectionId);
            }

            if(entity.Section != lastPost.Section)
            {
                if(lastPost.Section != null)
                {
                    lastPost.Section.CountOfUsage--;
                    repositoryOfSection.Update(lastPost.Section);
                }
                entity.Section.CountOfUsage++;
                repositoryOfSection.Update(entity.Section);
            }
            if(entity.Created.Millisecond == 0)
            {
                entity.Created = DateTime.Now;
            }
            serviceOfSearch.Update<PostEntity>(entity);
            entity = base.Update(entity, properties);
            return entity;
        }

        public override async Task Delete(PostEntity entity)
        {
            IRepository<SectionEntity> repositoryOfSection = new RepositoryOfSection(context);
            IRepository<CommentEntity> repositoryOfComment = new RepositoryOfComment(context, serviceOfSearch);
            IRepository<PostRatingEntity> repositoryOfPostRating = new RepositoryOfPostRating(context, serviceOfSearch);
            IRepository<PostTagEntity> repositoryOfPostTag = new RepositoryOfPostTag(context);
            var section = entity.Section;
            if (section == null)
            {
                section = repositoryOfSection.Read(a => a.Id == entity.SectionId);
            }
            if(section != null)
            {
                section.CountOfUsage--;
                repositoryOfSection.Update(section);
            }
            var comments = entity.Comments;
            if(comments == null)
            {
                comments = repositoryOfComment.ReadMany(new Expression<Func<CommentEntity, bool>>[] { a => a.PostId == entity.Id }, null);
            }
            comments = comments.ToList();
            foreach (var item in comments)
            {
                repositoryOfComment.Delete(item);
            }
            var postRatings = entity.PostRatings;
            if (postRatings == null)
            {
                postRatings = repositoryOfPostRating.ReadMany(new Expression<Func<PostRatingEntity, bool>>[] { a => a.PostId == entity.Id }, null);
            }
            postRatings = postRatings.ToList();
            foreach (var item in postRatings)
            {
                repositoryOfPostRating.Delete(item);
            }
            var postTags = entity.Tags;
            if(postTags == null)
            {
                postTags = repositoryOfPostTag.ReadMany(new Expression<Func<PostTagEntity, bool>>[] { a => a.PostId == entity.Id }, null);
            }
            postTags = postTags.ToList();
            foreach (var item in postTags)
            {
                repositoryOfPostTag.Delete(item);
            }
            serviceOfSearch.DeletePost(entity);
            await base.Delete(entity);
        }
    }
}
