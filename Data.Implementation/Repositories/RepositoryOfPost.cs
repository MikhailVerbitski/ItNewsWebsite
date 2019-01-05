using Data.Contracts.Models.Entities;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfPost : DefaultRepository<PostEntity>
    {
        public RepositoryOfPost(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public override PostEntity Create(PostEntity entity)
        {
            RepositoryOfSection repositoryOfSection = new RepositoryOfSection(context);

            var section = entity.Section;
            if(section == null)
            {
                section = repositoryOfSection.Read(a => a.Id == entity.SectionId);
            }
            section.CountOfUsage++;
            repositoryOfSection.Update(section);

            return base.Create(entity);
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
