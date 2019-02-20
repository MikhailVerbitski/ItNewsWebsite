using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfPost : DefaultRepository<PostEntity>
    {
        public RepositoryOfPost(ApplicationDbContext context) : base(context)
        { }

        public override PostEntity Create(PostEntity entity)
        {
            RepositoryOfSection repositoryOfSection = new RepositoryOfSection(context);

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

            return base.Create(entity);
        }

        public override void Update(PostEntity entity, params Expression<Func<PostEntity, object>>[] properties)
        {
            var lastPost = this.Read(a => a.Id == entity.Id, a => a.Section);
            RepositoryOfSection repositoryOfSection = new RepositoryOfSection(context);
            
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

            base.Update(entity, properties);
        }

        public override void Delete(PostEntity entity)
        {
            RepositoryOfSection repositoryOfSection = new RepositoryOfSection(context);
            RepositoryOfComment repositoryOfComment = new RepositoryOfComment(context);
            RepositoryOfPostRating repositoryOfPostRating = new RepositoryOfPostRating(context);
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
                comments = repositoryOfComment.ReadMany(new Expression<Func<CommentEntity, bool>>[] { a => a.PostId == entity.Id });
            }
            comments = comments.ToList();
            foreach (var item in comments)
            {
                repositoryOfComment.Delete(item);
            }
            var postRatings = entity.PostRatings;
            if (postRatings == null)
            {
                postRatings = repositoryOfPostRating.ReadMany(new Expression<Func<PostRatingEntity, bool>>[] { a => a.PostId == entity.Id });
            }
            postRatings = postRatings.ToList();
            foreach (var item in postRatings)
            {
                repositoryOfPostRating.Delete(item);
            }
            base.Delete(entity);
        }
    }
}
