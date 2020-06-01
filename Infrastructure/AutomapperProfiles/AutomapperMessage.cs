using AutoMapper;
using Data.Contracts.Models.Entities;
using Domain.Contracts.Models.ViewModels.Message;

namespace Infrastructure.AutomapperProfiles
{
    public class AutomapperMessage : Profile
    {
        public AutomapperMessage()
        {
            CreateMap<MessageEntity, MessageViewModel>()
                    .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                    .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                    .ForMember(a => a.ChatId, a => a.MapFrom(b => b.ChatRoomId))
                    .ForAllOtherMembers(a => a.Ignore());

            CreateMap<MessageViewModel, MessageEntity>()
                    .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                    .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                    .ForMember(a => a.ChatRoomId, a => a.MapFrom(b => b.ChatId))
                    .ForAllOtherMembers(a => a.Ignore());
        }
    }
}