using AutoMapper;
using Data.Contracts;
using Data.Contracts.Models.Entities;
using Search.Implementation;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Implementation.Services
{
    public class ServiceOfSection
    {
        private readonly IMapper mapper;
        private readonly IRepository<SectionEntity> repositoryOfSection;
        private readonly IRepository<PostEntity> repositoryOfPost;

        public ServiceOfSection(IMapper mapper, ServiceOfSearch serviceOfSearch, IRepository<SectionEntity> repositoryOfSection, IRepository<PostEntity> repositoryOfPost)
        {
            this.mapper = mapper;
            this.repositoryOfSection = repositoryOfSection;
            this.repositoryOfPost = repositoryOfPost;
        }

        public List<string> Get()
        {
            return repositoryOfSection.ReadMany().Select(a => a.Name).ToList();
        }
    }
}
