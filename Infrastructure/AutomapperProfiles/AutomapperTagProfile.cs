using AutoMapper;
using Data.Contracts.Models.Entities;
using Domain.Contracts.Models.ViewModels.Tag;

namespace Infrastructure.AutomapperProfiles
{
    public class AutomapperTagProfile : Profile
    {
        public AutomapperTagProfile()
        {
            CreateMap<TagEntity, TagViewModel>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Name, a => a.MapFrom(b => b.Name))
                .ForMember(a => a.CountOfUsage, a => a.MapFrom(b => b.CountOfUsage))
                .ForAllOtherMembers(a => a.Ignore());


            CreateMap<TagViewModel, TagEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Name, a => a.MapFrom(b => b.Name))
                .ForMember(a => a.CountOfUsage, a => a.MapFrom(b => b.CountOfUsage))
                .ForAllOtherMembers(a => a.Ignore());
        }
    }
}
