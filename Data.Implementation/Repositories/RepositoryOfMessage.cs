using Data.Contracts.Models.Entities;
using System;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfMessage : DefaultRepository<MessageEntity>
    {
        public RepositoryOfMessage(ApplicationDbContext context) : base(context)
        { }

        public MessageEntity Create(MessageEntity entity, string authorId, string[] users)
        {
            entity.Created = DateTime.Now;
            entity = base.Create(entity);
            return entity;
        }
    }
}
