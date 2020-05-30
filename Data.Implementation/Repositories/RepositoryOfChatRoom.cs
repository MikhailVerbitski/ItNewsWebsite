using Data.Contracts.Models.Entities;
using System;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfChatRoom : DefaultRepository<ChatRoomEntity>
    {
        public RepositoryOfChatRoom(ApplicationDbContext context) : base(context)
        {}

        public ChatRoomEntity Create(ChatRoomEntity entity, string authorId, string[] users)
        {
            entity.Created = DateTime.Now;
            var ChatRoom = base.Create(entity);
            return entity;
        }
    }
}
