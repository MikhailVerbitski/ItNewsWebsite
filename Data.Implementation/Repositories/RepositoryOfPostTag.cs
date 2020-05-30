using Data.Contracts;
using Data.Contracts.Models.Entities;
using System.Threading.Tasks;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfPostTag : DefaultRepository<PostTagEntity>
    {
        public RepositoryOfPostTag(ApplicationDbContext context) : base(context)
        { }

        public override PostTagEntity Create(PostTagEntity entity)
        {
            IRepository<TagEntity> repositoryOfTag = new RepositoryOfTag(context);

            var Tag = entity.Tag;
            if(Tag == null)
            {
                Tag = repositoryOfTag.Read(a => a.Id == entity.TagId);
            }
            Tag.CountOfUsage++;
            repositoryOfTag.Update(Tag);

            entity.Tag = null;
            return base.Create(entity);
        }

        public override async Task Delete(PostTagEntity entity)
        {
            IRepository<TagEntity> repositoryOfTag = new RepositoryOfTag(context);

            var Tag = entity.Tag;
            if (Tag == null)
            {
                Tag = repositoryOfTag.Read(a => a.Id == entity.TagId);
            }
            Tag.CountOfUsage--;
            repositoryOfTag.Update(Tag);

            await base.Delete(entity);
        }
    }
}
