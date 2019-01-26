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
                .ForMember(a => a.UserProfileId, a => a.MapFrom(b => b.UserProfileId))
                .ForMember(a => a.IsFinished, a => a.MapFrom(b => b.IsFinished))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<PostEntity, PostMiniViewModel>()
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForMember(a => a.Created, a => a.MapFrom(b => b.Created))
                .ForMember(a => a.Score, a => a.MapFrom(b => (b.CountOfScore == 0) ? 0 : (double)b.SumOfScore / b.CountOfScore))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<PostEntity, PostCompactViewModel>()
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForMember(a => a.Created, a => a.MapFrom(b => b.Created))
                .ForMember(a => a.Score, a => a.MapFrom(b => (b.CountOfScore == 0) ? 0 : (double)b.SumOfScore / b.CountOfScore))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<PostEntity, PostViewModel>()
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForMember(a => a.Section, a => a.MapFrom(b => (b.Section != null) ? b.Section.Name : null))
                .ForMember(a => a.SectionId, a => a.MapFrom(b => b.SectionId))
                .ForMember(a => a.Score, a => a.MapFrom(b => (b.CountOfScore == 0) ? 0 : (double)b.SumOfScore / b.CountOfScore))
                .ForMember(a => a.Created, a => a.MapFrom(b => b.Created))
                .ForAllOtherMembers(a => a.Ignore());

            CreateMap<PostCreateEditViewModel, PostEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForMember(a => a.SectionId, a => a.MapFrom(b => b.SectionId))
                .ForMember(a => a.UserProfileId, a => a.MapFrom(b => b.UserProfileId))
                .ForMember(a => a.IsFinished, a => a.MapFrom(b => b.IsFinished))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<PostMiniViewModel, PostEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.BriefDesctiption, a => a.MapFrom(b => b.BriefDesctiption))
                .ForMember(a => a.Header, a => a.MapFrom(b => b.Header))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<PostCompactViewModel, PostEntity>()
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
