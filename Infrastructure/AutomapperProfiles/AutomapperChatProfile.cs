using AutoMapper;
using Data.Contracts.Models.Entities;
using Domain.Contracts.Models.ViewModels.Message;

namespace Infrastructure.AutomapperProfiles
{
    public class AutomapperChatProfile : Profile
    {
        public AutomapperChatProfile()
        {
            CreateMap<ChatRoomEntity, ChatRoomViewModel>()
                    .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                    .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                    .ForAllOtherMembers(a => a.Ignore());

            CreateMap<UserChatEntity, ChatRoomViewModel>()
                   .ForMember(a => a.Id, a => a.MapFrom(b => b.ChatRoom.Id))
                   .ForMember(a => a.Header, a => a.MapFrom(b => b.ChatRoom.Header))
                   .ForAllOtherMembers(a => a.Ignore());

            CreateMap<ChatRoomViewModel, ChatRoomEntity>()
                    .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                    .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                    .ForAllOtherMembers(a => a.Ignore());
        }
    }
}
