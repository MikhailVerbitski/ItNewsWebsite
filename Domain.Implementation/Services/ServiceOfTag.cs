using AutoMapper;
using Data.Contracts.Models.Entities;
using Data.Implementation;
using Data.Implementation.Repositories;
using Domain.Contracts.Models.ViewModels.Tag;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Implementation.Services
{
    public class ServiceOfTag
    {
        private readonly IMapper mapper;
        private readonly RepositoryOfTag repositoryOfTag;
        private readonly RepositoryOfPostTag repositoryOfPostTag;

        public ServiceOfTag(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;
            repositoryOfTag = new RepositoryOfTag(context);
            repositoryOfPostTag = new RepositoryOfPostTag(context);
        }

        public List<TagViewModel> Get()
        {
            var tagEntities = repositoryOfTag.ReadMany(null);
            var tagViewModels = mapper.Map<IEnumerable<TagEntity>, IEnumerable<TagViewModel>>(tagEntities);
            return tagViewModels.ToList();
        }
        public IEnumerable<TagViewModel> GetTagsForPost(PostEntity postEntity)
        {
            var tags = postEntity.Tags.Select(a => repositoryOfTag.Read(b => b.Id == a.TagId)).Select(a => mapper.Map<TagEntity, TagViewModel>(a));
            return tags;
        }
        private IEnumerable<TagEntity> MapTagViewModelToTagEntity(IEnumerable<TagViewModel> tagViewModels) => tagViewModels.Select(a =>
            {
                var tag = mapper.Map<TagViewModel, TagEntity>(a);
                if (tag.Id == 0)
                {
                    tag = repositoryOfTag.Create(new TagEntity() { Name = a.Name });
                }
                return tag;
            });
        public List<PostTagEntity> AddTagsPost(IEnumerable<TagViewModel> tagViewModels, int postId) => MapTagViewModelToTagEntity(tagViewModels)
            .Select(a => repositoryOfPostTag.Create(new PostTagEntity()
            {
                PostId = postId,
                TagId = a.Id,
                Tag = a,
            }))
            .ToList();
        public IEnumerable<PostTagEntity> TagsPostUpdate(IEnumerable<TagViewModel> tagViewModels, IEnumerable<PostTagEntity> lastTagEntities, int postId)
        {
            var postTagEntities = MapTagViewModelToTagEntity(tagViewModels);
            lastTagEntities
                .Select(a => a.TagId.Value)
                .Except(postTagEntities.Select(a => a.Id))
                .Select(a => lastTagEntities.Single(b => b.Id == a))
                .ToList()
                .ForEach(a => repositoryOfPostTag.Delete(a));
            return postTagEntities
                .Select(a => a.Id)
                .Except(lastTagEntities.Select(a => a.TagId.Value))
                .Select(a => postTagEntities.Single(b => b.Id == a))
                .Select(a => repositoryOfPostTag.Create(new PostTagEntity()
                {
                    PostId = postId,
                    TagId = a.Id,
                    Tag = a,
                }))
                .ToList();
        }
    }
}