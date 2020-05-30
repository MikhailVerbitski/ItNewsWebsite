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
    public class ServiceOfChat
    {
        private readonly IMapper mapper;
        private readonly IRepository<ChatRoomEntity> repositoryOfChatRoom;
        private readonly IRepository<UserChatEntity> repositoryOfUserChat;
        private readonly IRepository<UserProfileEntity> repositoryOfUserProfile;
        private readonly ServiceOfUser serviceOfUser;


        public ServiceOfChat(
            IMapper mapper,
            ServiceOfUser serviceOfUser,
            IRepository<UserProfileEntity> repositoryOfUserProfile,
            IRepository<ChatRoomEntity> repositoryOfChatRoom,
            IRepository<UserChatEntity> repositoryOfUserChat
            )
        {
            this.mapper = mapper;
            this.serviceOfUser = serviceOfUser;
            this.repositoryOfChatRoom = repositoryOfChatRoom;
            this.repositoryOfUserProfile = repositoryOfUserProfile;
            this.repositoryOfUserChat = repositoryOfUserChat;
        }


        public async Task<ChatRoomViewModel> Create(ChatRoomViewModel chatRoomViewModel, string applicationUserIdCurrent, string[] otherUserIds)
        {
            var users = new string[] { applicationUserIdCurrent }.Concat(otherUserIds);
            var chatRoomEntity = mapper.Map<ChatRoomViewModel, ChatRoomEntity>(chatRoomViewModel);
            chatRoomEntity = repositoryOfChatRoom.Create(chatRoomEntity);
            chatRoomEntity.UserChats = users.Select(a => {
                var user = repositoryOfUserProfile.Read(b => b.ApplicationUserId == a);
                var UserChat = new UserChatEntity();
                UserChat.ChatRoom = chatRoomEntity;
                UserChat.ChatRoomId = chatRoomEntity.Id;
                UserChat.UserId = user.Id;
                UserChat.UserProfile = user;
                return repositoryOfUserChat.Create(UserChat);
            }).ToList();
            chatRoomViewModel = mapper.Map<ChatRoomEntity, ChatRoomViewModel>(chatRoomEntity);
            return chatRoomViewModel;
        }
        public async Task<ChatRoomViewModel> Update(string applicationUserIdCurrent, ChatRoomViewModel chatRoomViewModel)
        {
            var applicationUserIds = chatRoomViewModel.Users.Select(u => u.ApplicationUserId).Concat(new string[] { applicationUserIdCurrent }).ToList();
            var chatRoomEntity = mapper.Map<ChatRoomViewModel, ChatRoomEntity>(chatRoomViewModel);
            chatRoomEntity = repositoryOfChatRoom.Read(a => a.Id == chatRoomEntity.Id);
            // delete
            chatRoomEntity.UserChats    
                .TakeWhile(userChat => !applicationUserIds.Any(applicationUserId => applicationUserId == userChat.UserProfile.ApplicationUserId))
                .ToList()
                .ForEach(async a => await repositoryOfUserChat.Delete(a));
            // create
            chatRoomEntity.UserChats = applicationUserIds          
                .TakeWhile(applicationUserId => !chatRoomEntity.UserChats.Any(userChat => userChat.UserProfile.ApplicationUserId == applicationUserId))
                .Select(applicationUserId => {
                    var user = repositoryOfUserProfile.Read(b => b.ApplicationUserId == applicationUserId);
                    var UserChat = new UserChatEntity();
                    UserChat.ChatRoom = chatRoomEntity;
                    UserChat.ChatRoomId = chatRoomEntity.Id;
                    UserChat.UserId = user.Id;
                    UserChat.UserProfile = user;
                    return UserChat;
                });
            chatRoomEntity = repositoryOfChatRoom.Update(chatRoomEntity);
            chatRoomViewModel = mapper.Map<ChatRoomEntity, ChatRoomViewModel>(chatRoomEntity);
            return chatRoomViewModel;
        }
        public async Task Delete(string applicationUserIdCurrent, int ChatRoomId)
        {
            var chatRoomEntity = repositoryOfChatRoom.Read(a => a.Id == ChatRoomId);
            await repositoryOfChatRoom.Delete(chatRoomEntity);
        }
        public List<ChatRoomViewModel> Get(string type, string applicationUserIdCurrent, int? skip, int? take)
        {
            IEnumerable<UserChatEntity> userChatEntities = repositoryOfUserChat.ReadMany(
                new Expression<Func<UserChatEntity, bool>>[] { a => a.UserProfile.ApplicationUserId == applicationUserIdCurrent },
                    a => a.ChatRoom,
                    a => a.UserProfile);
            userChatEntities = (skip != null) ? userChatEntities.Skip(skip.Value) : userChatEntities;
            userChatEntities = (take != null) ? userChatEntities = userChatEntities.Take(take.Value) : userChatEntities;
            var chatRoomViewModel = userChatEntities
                .Select(a => mapper.Map<UserChatEntity, ChatRoomViewModel>(a))
                .AsParallel()
                .ToList();
            return chatRoomViewModel;
        }
    }
}
