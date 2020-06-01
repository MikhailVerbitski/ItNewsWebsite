using AutoMapper;
using Data.Contracts;
using Data.Contracts.Models.Entities;
using Domain.Contracts.Models.ViewModels.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain.Implementation.Services
{
    public class ServiceOfMessage
    {
        private readonly IMapper mapper;
        private readonly ServiceOfUser serviceOfUser;
        private readonly IRepository<MessageEntity> repositoryOfMessage;
        private readonly IRepository<UserProfileEntity> repositoryOfUserProfile;

        public ServiceOfMessage(
            IMapper mapper,
            ServiceOfUser serviceOfUser,
            IRepository<UserProfileEntity> repositoryOfUserProfile,
            IRepository<MessageEntity> repositoryOfMessage
            )
        {
            this.mapper = mapper;
            this.serviceOfUser = serviceOfUser;
            this.repositoryOfUserProfile = repositoryOfUserProfile;
            this.repositoryOfMessage = repositoryOfMessage;
        }

        public async Task<MessageViewModel> Create(string applicationUserIdCurrent, MessageViewModel messageViewModel)
        {
            var messageEntity = mapper.Map<MessageViewModel, MessageEntity>(messageViewModel);
            messageEntity.UserProfile = repositoryOfUserProfile.Read(b => b.ApplicationUserId == applicationUserIdCurrent);
            messageEntity = repositoryOfMessage.Create(messageEntity);
            messageViewModel = mapper.Map<MessageEntity, MessageViewModel>(messageEntity);
            return messageViewModel;
        }
        public async Task<MessageViewModel> Update(string applicationUserIdCurrent, MessageViewModel messageViewModel)
        {
            var messageEntity = mapper.Map<MessageViewModel, MessageEntity>(messageViewModel);
            messageEntity.UserProfile = repositoryOfUserProfile.Read(b => b.ApplicationUserId == applicationUserIdCurrent);
            messageEntity = repositoryOfMessage.Update(messageEntity);
            messageViewModel = mapper.Map<MessageEntity, MessageViewModel>(messageEntity);
            return messageViewModel;
        }
        public async Task Delete(string applicationUserIdCurrent, int MessageId)
        {
            var messageEntity = new MessageEntity() { Id = MessageId };
            await repositoryOfMessage.Delete(messageEntity);
        }
        public List<MessageViewModel> Get(string type, string applicationUserIdCurrent, int? skip, int? take, int chatId)
        {
            IEnumerable<MessageEntity> messages = repositoryOfMessage.ReadMany(
                new Expression<Func<MessageEntity, bool>>[] { a => a.ChatRoomId == chatId },
                "Created",
                new Expression<Func<MessageEntity, object>>[] { a => a.UserProfile })
                .Reverse();
            messages = (skip != null) ? messages.Skip(skip.Value) : messages;
            messages = (take != null) ? messages.Take(take.Value) : messages;
            var messagesViewModel = messages
                .Select(messageEntity =>
                {
                    var message = mapper.Map<MessageEntity, MessageViewModel>(messageEntity);
                    message.Author = serviceOfUser.GetUsers(a => a.UserProfileId == messageEntity.UserProfileId).First();
                    return message;
                })
                .AsParallel()
                .ToList();
            return messagesViewModel;
        }
    }
}
