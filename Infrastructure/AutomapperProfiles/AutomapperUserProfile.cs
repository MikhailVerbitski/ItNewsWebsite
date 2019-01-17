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
            CreateMap<ApplicationUserEntity, UserEditViewModel>()
                .ForMember(a => a.ApplicationUserId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.UserProfileId, a => a.MapFrom(b => b.UserProfileId))
                .ForMember(a => a.Login, a => a.MapFrom(b => b.UserName))
                .ForMember(a => a.Email, a => a.MapFrom(b => b.Email))
                .ForMember(a => a.PhoneNumber, a => a.MapFrom(b => b.PhoneNumber))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<ApplicationUserEntity, UserMiniViewModel>()
                .ForMember(a => a.ApplicationUserId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Avatar, a => a.MapFrom(b => b.Avatar))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.CountOfLikes, a => a.MapFrom(b => b.CountOfLikes))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<ApplicationUserEntity, UserViewModel>()
                .ForMember(a => a.ApplicationUserId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Avatar, a => a.MapFrom(b => b.Avatar))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.CountOfLikes, a => a.MapFrom(b => b.CountOfLikes))
                .ForAllOtherMembers(a => a.Ignore());

            CreateMap<UserEditViewModel, ApplicationUserEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.ApplicationUserId))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.UserProfileId, a => a.MapFrom(b => b.UserProfileId))
                .ForMember(a => a.UserName, a => a.MapFrom(b => b.Login))
                .ForMember(a => a.PasswordHash, a => a.MapFrom(b => b.Password))
                .ForMember(a => a.Email, a => a.MapFrom(b => b.Email))
                .ForMember(a => a.PhoneNumber, a => a.MapFrom(b => b.PhoneNumber))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<UserMiniViewModel, ApplicationUserEntity>()
                .ForMember(a => a.Avatar, a => a.MapFrom(b => b.Avatar))
                .ForMember(a => a.Id, a => a.MapFrom(b => b.ApplicationUserId))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<UserViewModel, ApplicationUserEntity>()
                .ForMember(a => a.Avatar, a => a.MapFrom(b => b.Avatar))
                .ForMember(a => a.Id, a => a.MapFrom(b => b.ApplicationUserId))
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForAllOtherMembers(a => a.Ignore());


            CreateMap<RegisterViewModel, ApplicationUserEntity>()
                .ForMember(a => a.FirstName, a => a.MapFrom(b => b.FirstName))
                .ForMember(a => a.LastName, a => a.MapFrom(b => b.LastName))
                .ForMember(a => a.UserName, a => a.MapFrom(b => b.Login))
                .ForMember(a => a.PasswordHash, a => a.MapFrom(b => b.Password))
                .ForMember(a => a.Email, a => a.MapFrom(b => b.Email))
                .ForAllOtherMembers(a => a.Ignore());
        }
    }
}
