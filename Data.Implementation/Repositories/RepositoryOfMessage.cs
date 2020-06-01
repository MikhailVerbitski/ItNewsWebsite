using Data.Contracts.Models.Entities;
using System;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfMessage : DefaultRepository<MessageEntity>
    {
        public RepositoryOfMessage(ApplicationDbContext context) : base(context)
        { }

        public override MessageEntity Create(MessageEntity entity)
        {
            entity.Created = DateTime.Now;
            entity = base.Create(entity);
            return entity;
        }
    }
}
