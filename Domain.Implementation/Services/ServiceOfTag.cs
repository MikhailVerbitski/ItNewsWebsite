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

        public ServiceOfTag(ApplicationDbContext context, IMapper mapper)
        {
            this.mapper = mapper;

            repositoryOfTag = new RepositoryOfTag(context);
        }

        public List<TagViewModel> Get()
        {
            var tagEntities = repositoryOfTag.ReadMany(null);
            var tagViewModels = mapper.Map<IEnumerable<TagEntity>, IEnumerable<TagViewModel>>(tagEntities);
            return tagViewModels.ToList();
        }
    }
}
