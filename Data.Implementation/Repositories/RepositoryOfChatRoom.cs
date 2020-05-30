using Data.Contracts;
using Data.Contracts.Models.Entities;
using System;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfChatRoom : DefaultRepository<ChatRoomEntity>
    {
        IRepository<UserChatEntity> repositoryOfUserChat;
        IRepository<UserProfileEntity> repositoryOfUserProfile;

        public RepositoryOfChatRoom(
            ApplicationDbContext context, 
            IRepository<UserChatEntity> repositoryOfUserChat,
            IRepository<UserProfileEntity> repositoryOfUserProfile
            ) : base(context)
        {
            this.repositoryOfUserChat = repositoryOfUserChat;
            this.repositoryOfUserProfile = repositoryOfUserProfile;
        }

        public ChatRoomEntity Create(ChatRoomEntity entity, string authorId, string[] users)
        {
            entity.Created = DateTime.Now;
            var ChatRoom = base.Create(entity);
            return entity;
        }
    }
}
