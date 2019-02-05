using AutoMapper;
using Data.Contracts.Models.Entities;
using Domain.Contracts.Models.ViewModels.Account;
using Domain.Contracts.Models.ViewModels.User;

namespace Infrastructure.AutomapperProfiles
{
    public class AutomapperUserProfile : Profile
    {
        public AutomapperUserProfile()
        {
            CreateMap<ApplicationUserEntity, UserMiniViewModel>()
                .ForMember(a => a.ApplicationUserId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Avatar, a => a.MapFrom(b => b.Avatar))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.Login, a => a.MapFrom(b => b.UserName))
                .ForMember(a => a.Created, a => a.MapFrom(b => b.Created))
                .ForMember(a => a.CountOfLikes, a => a.MapFrom(b => b.CountOfLikes))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<ApplicationUserEntity, UserViewModel>()
                .ForMember(a => a.ApplicationUserId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Avatar, a => a.MapFrom(b => b.Avatar))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.CountOfLikes, a => a.MapFrom(b => b.CountOfLikes))
                .ForMember(a => a.Email, a => a.MapFrom(b => b.Email))
                .ForMember(a => a.Login, a => a.MapFrom(b => b.UserName))
                .ForMember(a => a.Created, a => a.MapFrom(b => b.Created))
                .ForAllOtherMembers(a => a.Ignore());
            
            CreateMap<UserMiniViewModel, ApplicationUserEntity>()
                .ForMember(a => a.Avatar, a => a.MapFrom(b => b.Avatar))
                .ForMember(a => a.Id, a => a.MapFrom(b => b.ApplicationUserId))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.UserName, a => a.MapFrom(b => b.Login))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<UserViewModel, ApplicationUserEntity>()
                .ForMember(a => a.Avatar, a => a.MapFrom(b => b.Avatar))
                .ForMember(a => a.Id, a => a.MapFrom(b => b.ApplicationUserId))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.Email, a => a.MapFrom(b => b.Email))
                .ForMember(a => a.UserName, a => a.MapFrom(b => b.Login))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<UserUpdateViewModel, ApplicationUserEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.ApplicationUserId))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.UserName, a => a.MapFrom(b => b.Login))
                .ForMember(a => a.Email, a => a.MapFrom(b => b.Email))
                .ForAllOtherMembers(a => a.Ignore());


            CreateMap<RegisterViewModel, ApplicationUserEntity>()
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.UserName, a => a.MapFrom(b => b.Login))
                .ForMember(a => a.Email, a => a.MapFrom(b => b.Email))
                .ForAllOtherMembers(a => a.Ignore());

        }
    }
}
