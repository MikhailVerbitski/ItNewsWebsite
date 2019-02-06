using AutoMapper;
using Data.Contracts.Models.Entities;
using Domain.Contracts.Models;

namespace Infrastructure.AutomapperProfiles
{
    public class AutomapperRoleProfile : Profile
    {
        public AutomapperRoleProfile()
        {
            CreateMap<UserRole, RoleEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Name, a => a.MapFrom(b => b.Name))
                .ForMember(a => a.Color, a => a.MapFrom(b => b.Color))
                .ForAllOtherMembers(a => a.Ignore());

            CreateMap<RoleEntity, UserRole>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Name, a => a.MapFrom(b => b.Name))
                .ForMember(a => a.Color, a => a.MapFrom(b => b.Color))
                .ForAllOtherMembers(a => a.Ignore());
        }
    }
}
