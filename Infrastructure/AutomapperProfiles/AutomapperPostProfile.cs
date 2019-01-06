using AutoMapper;
using Data.Contracts.Models.Entities;
using Domain.Contracts.Models.ViewModels.Post;

namespace Infrastructure.AutomapperProfiles
{
    public class AutomapperPostProfile : Profile
    {
        public AutomapperPostProfile()
        {
            CreateMap<PostEntity, PostCreateEditViewModel>()
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForMember(a => a.SectionId, a => a.MapFrom(b => b.SectionId))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<PostEntity, PostMiniViewModel>()
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForMember(a => a.Score, a => a.MapFrom(b => b.SumOfScore / b.CountOfScore))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<PostEntity, PostViewModel>()
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForMember(a => a.Score, a => a.MapFrom(b => b.SumOfScore / b.CountOfScore))
                .ForAllOtherMembers(a => a.Ignore());

            CreateMap<PostCreateEditViewModel, PostEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForMember(a => a.SectionId, a => a.MapFrom(b => b.SectionId))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<PostMiniViewModel, PostEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<PostViewModel, PostEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForAllOtherMembers(a => a.Ignore());
        }
    }
}
