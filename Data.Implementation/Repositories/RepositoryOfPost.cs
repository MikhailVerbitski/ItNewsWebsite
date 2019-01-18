using System;
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

            return base.Create(entity);
        }

        public override void Update(PostEntity entity, params Expression<Func<PostEntity, object>>[] properties)
        {
            var lastPost = this.Read(a => a.Id == entity.Id, a => a.Section);
            RepositoryOfSection repositoryOfSection = new RepositoryOfSection(context);

            if(entity.Section != null && lastPost.Section != null && entity.Section != lastPost.Section)
            {
                lastPost.Section.CountOfUsage--;
                repositoryOfSection.Update(lastPost.Section);
            }

            if (entity.Section == null)
            {
                var section = repositoryOfSection.Read(a => a.Id == entity.SectionId);
                if (section != null)
                {
                    section.CountOfUsage++;
                    repositoryOfSection.Update(section);
                }
            }
            else
            {
                entity.Section.CountOfUsage++;
                repositoryOfSection.Update(entity.Section);
            }

            if (lastPost.Section == null)
            {
                var section = repositoryOfSection.Read(a => a.Id == lastPost.SectionId);
                if (section != null)
                {
                    section.CountOfUsage++;
                    repositoryOfSection.Update(section);
                }
            }
            else
            {
                lastPost.Section.CountOfUsage++;
                repositoryOfSection.Update(lastPost.Section);
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

            var section = entity.Section;
            if (section == null)
            {
                section = repositoryOfSection.Read(a => a.Id == entity.SectionId);
            }
            section.CountOfUsage--;
            repositoryOfSection.Update(section);

            base.Delete(entity);
        }
    }
}
