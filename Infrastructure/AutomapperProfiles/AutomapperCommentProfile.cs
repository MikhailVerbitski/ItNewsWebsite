using AutoMapper;
using Data.Contracts.Models.Entities;
using Domain.Contracts.Models.ViewModels.Comment;
using Search.Contracts.Models;

namespace Infrastructure.AutomapperProfiles
{
    public class AutomapperCommentProfile : Profile
    {
        public AutomapperCommentProfile()
        {
            CreateMap<CommentEntity, CommentCreateEditViewModel>()
                .ForMember(a => a.CommentId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.PostId))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<CommentEntity, CommentMiniViewModel>()
                .ForMember(a => a.CommentId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.CountOfLikes, a => a.MapFrom(b => b.CountOfLikes))
                .ForMember(a => a.Created, a => a.MapFrom(b => b.Created))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<CommentEntity, CommentViewModel>()
                .ForMember(a => a.CommentId, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.CountOfLikes, a => a.MapFrom(b => b.CountOfLikes))
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.Created, a => a.MapFrom(b => b.Created))
                .ForAllOtherMembers(a => a.Ignore());

            CreateMap<CommentCreateEditViewModel, CommentEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.CommentId))
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<CommentMiniViewModel, CommentEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.CommentId))
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForMember(a => a.CountOfLikes, a => a.MapFrom(b => b.CountOfLikes))
                .ForAllOtherMembers(a => a.Ignore());
            CreateMap<CommentViewModel, CommentEntity>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.CommentId))
                .ForMember(a => a.PostId, a => a.MapFrom(b => b.PostId))
                .ForMember(a => a.CountOfLikes, a => a.MapFrom(b => b.CountOfLikes))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content))
                .ForAllOtherMembers(a => a.Ignore());

            CreateMap<CommentEntity, CommentSearch>()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content));
            CreateMap<CommentSearch, CommentEntity> ()
                .ForMember(a => a.Id, a => a.MapFrom(b => b.Id))
                .ForMember(a => a.Content, a => a.MapFrom(b => b.Content));
        }
    }
}
