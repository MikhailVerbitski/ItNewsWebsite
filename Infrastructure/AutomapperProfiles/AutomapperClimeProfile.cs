using AutoMapper;
using Domain.Contracts.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.AutomapperProfiles
{
    public class AutomapperClimeProfile : Profile
    {
        public AutomapperClimeProfile()
        {
            CreateMap<IdentityUserClaim<string>, UserClaim>();
            CreateMap<UserClaim, IdentityUserClaim<string>>();
        }
    }
}
